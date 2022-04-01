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
// [x] Delay
// [x] Stack Inspector
// [x] Effect Inspector
// [x] Tooltips
// [x] Event trigger
// [x] Events
// [x] Create a struct to handle array of effects
// [x] Stack looping
// [x] Test stack events
// [x] Create a simple C# API
//	   [x] play for stacks and effects
//	   [x] add and remove stacks + effects
// [x] Effect Inspector Resizing bug
// [x] Comments, warnings and such
// [ ] Demos + Documentation
// [ ] Export for Legacy, URP, and HDRP

public enum CinemaestreEffectType { SLIDE, PAN, ZOOM, FADE, DELAY }
public enum SlideType { WORLD_POS, LOCAL_OFFSET, DIR_AND_MAG }
public enum PanDirection { HORIZONTAL, VERTICAL }

#region CINEMAESTRE EFFECTS
[System.Serializable]
public class CinemaestreEffect {
	public CinemaestreEffectType effectType;

	public float duration = 1f;

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
	public Vector3 panAxisOfRotation; 
	public bool panGlobalSpace;
	public float panAngle;

	public float zoomTargetFOV;
	public float zoomInitialFOV;

	public Color fadeColor;
	public bool fadeOut = true;

	public UnityEvent OnEffectComplete;
}
#endregion

#region CINEMAESTRE STACK
[System.Serializable]
public class CinemaestreStack {
	public bool autoplay;

	public bool loop = false;
	public bool loopForever = true; 
	public int iterations = 1;
	public int timesIterated = 0;
	public bool pingpong = false; 

	public UnityEvent OnStart;
	public UnityEvent OnLoop;
	public UnityEvent OnComplete;

	public CinemaestreEffect[] effects;
}
#endregion

namespace Cinemaestre {
	public class CinemaestreCamera : MonoBehaviour {
		/// <summary>
		/// List of the CinemaestreStacks. Manipulate this manually, using the API, or through the editor interface. 
		/// </summary>
		public List<CinemaestreStack> stacks = new List<CinemaestreStack>();

		Image fadePanel; 

		#region UNITY FUNCTIONS
		void Start() {
			for (int i=0; i<stacks.Count; i++) {
				if (stacks[i].autoplay) PlayEffectStack(i);
			}
		}
		#endregion

		#region API
		/// <summary>
		/// Play the entire effect stack denoted by index
		/// </summary>
		/// <param name="index"></param>
		public Coroutine PlayEffectStack(int index) {
			return StartCoroutine(PlayEffectStackImpl(index));
		}

		/// <summary>
		/// This will play a given CinemaestreEffect. Returns a coroutine to control the process.
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		public Coroutine PlayEffect(CinemaestreEffect effect) {
			return StartCoroutine(PlayEffectImpl(effect));
		}

		/// <summary>
		/// Append a stack to the stack list. Returns the stack index.
		/// </summary>
		/// <param name="stack"></param>
		public int AddStack(CinemaestreStack stack) {
			stacks.Add(stack);
			return stacks.Count - 1;
		}

		/// <summary>
		/// Insert an effect into the specified stack at the given index. 
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="stackIndex"></param>
		/// <param name="effectIndex"></param>
		public void InsertEffect(CinemaestreEffect effect, int stackIndex, int effectIndex) {
			CinemaestreEffect[] effects = stacks[stackIndex].effects;
			CinemaestreEffect[] newEffects = new CinemaestreEffect[effects.Length + 1];

			for (int i=0; i<newEffects.Length; i++) {
				if (i == effectIndex) newEffects[i] = effect;
				else if (i < effectIndex) newEffects[i] = effects[i];
				else if (i > effectIndex) newEffects[i + 1] = effects[i];
			}

			stacks[stackIndex].effects = newEffects;
		}

		/// <summary>
		/// Remove a stack at the given index.
		/// </summary>
		/// <param name="stackIndex"></param>
		public void RemoveStack(int stackIndex) {
			stacks.RemoveAt(stackIndex);
		}

		/// <summary>
		/// Remove an effect at the given stack and effect index. 
		/// </summary>
		/// <param name="stackIndex"></param>
		/// <param name="effectIndex"></param>
		public void RemoveEffect(int stackIndex, int effectIndex) {
			CinemaestreEffect[] effects = stacks[stackIndex].effects;
			CinemaestreEffect[] newEffects = new CinemaestreEffect[effects.Length - 1];

			for (int i=0; i<newEffects.Length; i++) {
				if (i < effectIndex) newEffects[i] = effects[i];
				else newEffects[i] = effects[i+1];
			}

			stacks[stackIndex].effects = newEffects;
		}
		#endregion

		#region IMPLEMENTATIONS
		IEnumerator PlayEffectStackImpl(int index) {
			stacks[index].timesIterated = 0;
			stacks[index].OnStart.Invoke();

			bool loopAgain = true;
			bool forward = true;
			while (loopAgain) {
				loopAgain = false;

				if (forward) {
					for (int i = 0; i < stacks[index].effects.Length; i++) {
						yield return PlayEffectImpl(stacks[index].effects[i]);
						stacks[index].effects[i].OnEffectComplete.Invoke();
					}
				} else { 
					for (int i = stacks[index].effects.Length-1; i>=0; i--) { 
						yield return PlayEffectImpl(stacks[index].effects[i], forward: false); 
						stacks[index].effects[i].OnEffectComplete.Invoke();
					}
				}

				stacks[index].timesIterated++;

				if (stacks[index].loop) {
					if (stacks[index].loopForever) {
						loopAgain = true;
					} else if (stacks[index].timesIterated < stacks[index].iterations) {
						loopAgain = true;
					}

					if (stacks[index].pingpong && stacks[index].timesIterated % 2 == 1) forward = false;
					else forward = true;
				}

				if (loopAgain) stacks[index].OnLoop.Invoke();
			}

			stacks[index].OnComplete.Invoke();
		}

		IEnumerator PlayEffectImpl(CinemaestreEffect effect, bool forward=true) {
			LTDescr lt;

			if (effect.effectType == CinemaestreEffectType.SLIDE) {
				lt = GetSlideLT(effect, forward);
			} else if (effect.effectType == CinemaestreEffectType.PAN) {
				lt = GetPanLT(effect, forward);
			} else if (effect.effectType == CinemaestreEffectType.ZOOM) {
				lt = GetZoomLT(effect, forward);
			} else if (effect.effectType == CinemaestreEffectType.FADE) {
				lt = GetFadeLT(effect, forward);
			} else if (effect.effectType == CinemaestreEffectType.DELAY) {
				lt = GetDelayLT(effect, forward);
			} else { lt = null; } // this is not good haha

			if (effect.customEase) { 
				lt.setEase(effect.easeAnimationCurve);
			} else {
				lt.setEase(effect.easeType);
			}

			bool finished = false;
			lt.setOnComplete(() => { finished = true; });

			while (!finished) yield return null;
		}
		#endregion

		#region TWEEN FUNCTIONS
		LTDescr GetSlideLT(CinemaestreEffect effect, bool forward) {
			Vector3 initialPos = transform.position;

			if (effect.slideType == SlideType.WORLD_POS) {
				effect.slideMoveDir = (effect.slideWorldPos - initialPos).normalized;
				effect.slideMoveDistance = (effect.slideWorldPos - initialPos).magnitude;
			} else if (effect.slideType == SlideType.LOCAL_OFFSET) {
				effect.slideMoveDir = effect.slideLocalOffset.normalized;
				effect.slideMoveDistance = effect.slideLocalOffset.magnitude;
			}

			Vector3 moveDir = forward ? effect.slideMoveDir : -effect.slideMoveDir;

			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					transform.position = initialPos + moveDir * (effect.slideMoveDistance * value);
				});
		}

		LTDescr GetPanLT(CinemaestreEffect effect, bool forward) {
			Vector3 forwardVec = effect.panGlobalSpace ? effect.panAxisOfRotation : transform.rotation * effect.panAxisOfRotation; 

			if (!effect.panCustomDirection) {
				if (effect.panDirection == PanDirection.HORIZONTAL) {
					forwardVec = effect.panGlobalSpace ? Vector3.up : transform.up;
				} else if (effect.panDirection == PanDirection.VERTICAL) {
					forwardVec = effect.panGlobalSpace ? Vector3.right : transform.right;
				}
			}

			float panAngle = forward ? effect.panAngle : -effect.panAngle;
			return LeanTween.rotateAround(gameObject, forwardVec, panAngle, effect.duration);
		}

		LTDescr GetZoomLT(CinemaestreEffect effect, bool forward) { 
			Camera cam = GetComponent<Camera>();
			if (forward) effect.zoomInitialFOV = cam.fieldOfView;
			float initialFOV = cam.fieldOfView;
			float targetFOV = forward ? effect.zoomTargetFOV : effect.zoomInitialFOV;

			// TODO: double check this works for loops
			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					cam.fieldOfView = initialFOV + (effect.zoomTargetFOV - initialFOV) * value;
				});
		}

		LTDescr GetFadeLT(CinemaestreEffect effect, bool forward) {
			fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
			if (fadePanel == null) {
				Debug.LogError("Error: To use the fade effect you must have the CinemaestreFadeCanvas in the scene.");
				return null;
			}

			bool fadeOut = forward ? effect.fadeOut : !effect.fadeOut;

			if (fadeOut) effect.fadeColor.a = 0f;
			else effect.fadeColor.a = 1f;

			fadePanel.color = effect.fadeColor;

			return LeanTween.value(0f, 1f, effect.duration)
				.setOnUpdate((float value) => {
					Color c = fadePanel.color;
					c.a = fadeOut ? value : 1f - value;
					fadePanel.color = c;
				});
		}

		LTDescr GetDelayLT(CinemaestreEffect effect, bool forward) {
			return LeanTween.value(0f, 1f, effect.duration);
		}
		#endregion
	}
}

