using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TODO: 
// [x] Pan (in cardinal directions)
// [x] Zoom (in and out with an adjustable time rate)
// [x] Fade Screen (using hdrp)
// [x] Lerp FOV  (in and out with an adjustable time rate)
// [x] Inspector
// [x] Tooltips
// [x] Event trigger
// [ ] Create a struct for each effect that is subclassed, and can be added in an array to layer them
// [ ] Create a simple C# API
// [ ] Comment code, debug warnings and such
// [ ] Demo + Documentation

// Further development
// [ ] Slide splining
// [ ] Scene handles for rotations
// [ ] Fade render pass
// [ ] Timeline for animating effects
// [ ] Pan around a target object

namespace Cinemaestre {
	public enum CameraEffect { SLIDE, PAN, ZOOM, FADE, DELAY }

	public enum SlideType { WORLD_POS, LOCAL_OFFSET, DIR_AND_MAG }
	public enum PanDirection { HORIZONTAL, VERTICAL }

	#region CINEMAESTRE EFFECTS
	[System.Serializable]
	public class CinemaestreEffect {
		public CameraEffect effect;

		public float duration = 1f;

		public bool loop = false;
		public int iterations = 1; // this should not show up if loop is checked, and loopForever is not checked
		public bool loopForever = true; // should only show up if loop is checked
		public bool pingpong = false; // this should only show up if loop is checked

		public LeanTweenType easeType;
		public bool customEase = false;
		public AnimationCurve easeAnimationCurve; // only show this if customEase is true
	}
	#endregion

	public class CinemaestreCamera : MonoBehaviour {
		[HideInInspector] public UnityEvent Activate; // TODO: better name pls

		public CinemaestreEffect effect;


		public CinemaestreEffect[] effects;

		#region FIELDS
		[HideInInspector] public CameraEffect cameraEffect;

		[Header("General")]
		public float duration = 1f;
		public bool autoplay = false; 

		[Header("Looping")]
		public bool loop = false;
		public int iterations = 1; // this should not show up if loop is checked, and loopForever is not checked
		public bool loopForever = true; // should only show up if loop is checked
		public bool pingpong = false; // this should only show up if loop is checked

		[Header("Ease Function")]
		public LeanTweenType easeType;
		public bool customEase = false;
		public AnimationCurve easeAnimationCurve; // only show this if customEase is true

		[Header("Slide")]
		public SlideType slideType;
		public Vector3 slideWorldPos; // only show if worldpos
		public Vector3 slideLocalOffset; // only show if local offset
		public Vector3 slideMoveDir; // only show if movedir
		public float slideMoveDistance;

		[Header("Pan")]
		public PanDirection panDirection;
		public bool panCustomDirection = false;
		public Vector3 panAxisOfRotation; // only show this if custom direction is true
		public bool panGlobalSpace;
		public float panAngle;

		[Header("Zoom")]
		public float zoomTargetFOV;

		[Header("Fade")]
		public Color fadeColor;
		public bool fadeOut = true;
		Image fadePanel;

		#endregion

		#region UNITY FUNCTIONS
		void Start() {
			if (autoplay) PlayCameraEffects();
		}

		void OnEnable() {
			Activate.AddListener(PlayCameraEffects);
		}

		void OnDisable() {
			Activate.RemoveListener(PlayCameraEffects);
		}
		#endregion

		public UnityEvent OnStart;
		public UnityEvent OnLoop;
		public UnityEvent OnComplete;

		/// <summary>
		/// This will begin playing the CameraEffects
		/// </summary>
		public void PlayCameraEffects() {
			OnStart.Invoke();

			LTDescr lt;

			if (cameraEffect == CameraEffect.SLIDE) {
				lt = GetSlideLT();
			} else if (cameraEffect == CameraEffect.PAN) {
				lt = GetPanLT();
			} else if (cameraEffect == CameraEffect.ZOOM) {
				lt = GetZoomLT();
			} else if (cameraEffect == CameraEffect.FADE) {
				lt = GetFadeLT();
			} else { lt = null; } // this is not good haha

			if (loop) {
				if (!loopForever) {
					if (iterations == 0) { // dont permit 0 iterations because that will be interpreted as infinite
						lt.setLoopCount(1);
					} else {
						lt.setLoopCount(iterations);
					}
				}

				if (pingpong) {
					lt.setLoopPingPong();
				} else {
					lt.setLoopClamp();
				}
			}

			if (customEase) { 
				lt.setEase(easeAnimationCurve);
			} else {
				lt.setEase(easeType);
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

		#region TWEEN FUNCTIONS
		LTDescr GetSlideLT() {
			Vector3 initialPos = transform.position;

			if (slideType == SlideType.WORLD_POS) {
				slideMoveDir = (slideWorldPos - initialPos).normalized;
				slideMoveDistance = (slideWorldPos - initialPos).magnitude;
			} else if (slideType == SlideType.LOCAL_OFFSET) {
				slideMoveDir = slideLocalOffset.normalized;
				slideMoveDistance = slideLocalOffset.magnitude;
			} 

			return LeanTween.value(0f, 1f, duration)
				.setOnUpdate((float value) => {
					transform.position = initialPos + slideMoveDir * (slideMoveDistance * value);
				});
		}

		LTDescr GetPanLT() {
			Vector3 forward = panGlobalSpace ?  panAxisOfRotation : transform.rotation * panAxisOfRotation; 

			if (!panCustomDirection) {
				if (panDirection == PanDirection.HORIZONTAL) {
					forward = panGlobalSpace ? Vector3.up : transform.up;
				} else if (panDirection == PanDirection.VERTICAL) {
					forward = panGlobalSpace ? Vector3.right : transform.right;
				}
			}

			return LeanTween.rotateAround(gameObject, forward, panAngle, duration);
		}

		LTDescr GetZoomLT() { 
			Camera cam = GetComponent<Camera>();
			float initialFOV = cam.fieldOfView;

			return LeanTween.value(0f, 1f, duration)
				.setOnUpdate((float value) => {
					cam.fieldOfView = initialFOV + (zoomTargetFOV - initialFOV) * value;
				});
		}

		LTDescr GetFadeLT() {
			fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();

			if (fadeOut) fadeColor.a = 0f;
			else fadeColor.a = 1f;

			fadePanel.color = fadeColor;

			return LeanTween.value(0f, 1f, duration)
				.setOnUpdate((float value) => {
					Color c = fadePanel.color;
					c.a = fadeOut ? value : 1f - value;
					fadePanel.color = c;
				});
		}
		#endregion
	}
}

