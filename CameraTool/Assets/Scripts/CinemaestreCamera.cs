using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TODO: 
// [x] Pan (in cardinal directions)
// [x] Zoom (in and out with an adjustable time rate)
// [x] Fade Screen (using hdrp)
// [x] Lerp FOV (in and out with an adjustable time rate)
// [ ] Delay
// [x] Stack Inspector
// [ ] Effect Inspector
// [x] Tooltips
// [x] Event trigger
// [x] Events
// [x] Create a struct to handle array of effects
// [ ] Effect Stack implementations
// play, pause, stop
// for all and by index
// loop on a stack basis
// [ ] Create a simple C# API
// [ ] Comments, debug warnings and such
// [ ] Demos + Documentation

// Further development
// [ ] Slide splining
// [ ] Scene handles for rotations
// [ ] Fade render pass
// [ ] Pan around a target object
// [ ] Replace leantween with custom ease functions: https://easings.net/

public enum CinemaestreEffectType { SLIDE, PAN, ZOOM, FADE, DELAY }
public enum SlideType { WORLD_POS, LOCAL_OFFSET, DIR_AND_MAG }
public enum PanDirection { HORIZONTAL, VERTICAL }

#region CINEMAESTRE EFFECTS
[System.Serializable]
public class CinemaestreEffect {
	public CinemaestreEffectType effectType;

	public float duration = 1f;

	public bool loop = false;
	public int iterations = 1;
	public bool loopForever = true; 
	public bool pingpong = false; 

	public LeanTweenType easeType;
	public bool customEase = false;
	public AnimationCurve easeAnimationCurve; 

	public SlideType slideType;
	public Vector3 slideWorldPos; 
	public Vector3 slideLocalOffset;
	public Vector3 slideMoveDir; 
	public float slideMoveDistance;

	public PanDirection panDirection;
	public bool panCustomDirection = false;
	public Vector3 panAxisOfRotation; // only show this if custom direction is true
	public bool panGlobalSpace;
	public float panAngle;

	public float zoomTargetFOV;

	public Color fadeColor;
	public bool fadeOut = true;
}
#endregion

#region CINEMAESTRE STACK
[System.Serializable]
public class CinemaestreStack {
	public CinemaestreEffect[] effects; // each stack knows how to update on its own
}
#endregion

namespace Cinemaestre {
	public class CinemaestreCamera : MonoBehaviour {
		[HideInInspector] public UnityEvent Activate; // TODO: better name pls

		public List<CinemaestreStack> stacks = new List<CinemaestreStack>();

		#region FIELDS
		[Header("General")]
		public bool autoplay = false; 

		//[Header("Looping")]
		//public bool loop = false;
		//public int iterations = 1; // this should not show up if loop is checked, and loopForever is not checked
		//public bool loopForever = true; // should only show up if loop is checked
		//public bool pingpong = false; // this should only show up if loop is checked

		Image fadePanel;

		public UnityEvent OnStart;
		public UnityEvent OnLoop;
		public UnityEvent OnComplete;
		#endregion

		#region UNITY FUNCTIONS
		void Start() {
			if (autoplay) PlayEffectSequence();
		}

		void OnEnable() {
			Activate.AddListener(PlayEffectSequence);
		}

		void OnDisable() {
			Activate.RemoveListener(PlayEffectSequence);
		}
		#endregion

		#region API
		/// <summary>
		/// This will play the entire CinemaestreEffect sequence
		/// </summary>
		public void PlayEffectSequence() {
			for (int i=0; i<stacks[0].effects.Length; i++) {
				PlayEffect(stacks[0].effects[i]); 
			}
		}

		IEnumerator PlayEffectSequenceImpl() {
			yield return null;
		}

		/// <summary>
		/// This will play a given CinemaestreEffect
		/// </summary>
		public void PlayEffect(CinemaestreEffect effect) {
			OnStart.Invoke();

			LTDescr lt;

			if (effect.effectType == CinemaestreEffectType.SLIDE) {
				lt = GetSlideLT(effect);
			} else if (effect.effectType == CinemaestreEffectType.PAN) {
				lt = GetPanLT(effect);
			} else if (effect.effectType == CinemaestreEffectType.ZOOM) {
				lt = GetZoomLT(effect);
			} else if (effect.effectType == CinemaestreEffectType.FADE) {
				lt = GetFadeLT(effect);
			} else { lt = null; } // this is not good haha

			if (effect.loop) {
				if (!effect.loopForever) {
					if (effect.iterations == 0) { // dont permit 0 iterations because that will be interpreted as infinite
						lt.setLoopCount(1);
					} else {
						lt.setLoopCount(effect.iterations);
					}
				}

				if (effect.pingpong) {
					lt.setLoopPingPong();
				} else {
					lt.setLoopClamp();
				}
			}

			if (effect.customEase) { 
				lt.setEase(effect.easeAnimationCurve);
			} else {
				lt.setEase(effect.easeType);
			}

			lt.setOnComplete(() => {
				if (lt.loopCount == 0) {
					OnComplete.Invoke();
				} else {
					OnLoop.Invoke();
				}
			});

			lt.setOnCompleteOnRepeat(true);
		}
		#endregion

		#region TWEEN FUNCTIONS
		LTDescr GetSlideLT(CinemaestreEffect effect) {
			Vector3 initialPos = transform.position;

			if (effect.slideType == SlideType.WORLD_POS) {
				effect.slideMoveDir = (effect.slideWorldPos - initialPos).normalized;
				effect.slideMoveDistance = (effect.slideWorldPos - initialPos).magnitude;
			} else if (effect.slideType == SlideType.LOCAL_OFFSET) {
				effect.slideMoveDir = effect.slideLocalOffset.normalized;
				effect.slideMoveDistance = effect.slideLocalOffset.magnitude;
			} 

			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					transform.position = initialPos + effect.slideMoveDir * (effect.slideMoveDistance * value);
				});
		}

		LTDescr GetPanLT(CinemaestreEffect effect) {
			Vector3 forward = effect.panGlobalSpace ? effect.panAxisOfRotation : transform.rotation * effect.panAxisOfRotation; 

			if (!effect.panCustomDirection) {
				if (effect.panDirection == PanDirection.HORIZONTAL) {
					forward = effect.panGlobalSpace ? Vector3.up : transform.up;
				} else if (effect.panDirection == PanDirection.VERTICAL) {
					forward = effect.panGlobalSpace ? Vector3.right : transform.right;
				}
			}

			return LeanTween.rotateAround(gameObject, forward, effect.panAngle, effect.duration);
		}

		LTDescr GetZoomLT(CinemaestreEffect effect) { 
			Camera cam = GetComponent<Camera>();
			float initialFOV = cam.fieldOfView;

			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					cam.fieldOfView = initialFOV + (effect.zoomTargetFOV - initialFOV) * value;
				});
		}

		LTDescr GetFadeLT(CinemaestreEffect effect) {
			fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();

			if (effect.fadeOut) effect.fadeColor.a = 0f;
			else effect.fadeColor.a = 1f;

			fadePanel.color = effect.fadeColor;

			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					Color c = fadePanel.color;
					c.a = effect.fadeOut ? value : 1f - value;
					fadePanel.color = c;
				});
		}
		#endregion
	}
}

