using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// TODO: 
// [x] Pan (in cardinal directions)
// [x] Zoom (in and out with an adjustable time rate)
// [x] Fade Screen (using hdrp)
// [x] Lerp FOV  (in and out with an adjustable time rate)
// [ ] Inspector
// [ ] Tooltips
// [ ] Event trigger
// [ ] Create a struct for each effect that is subclassed, and can be added in an array to layer them
// [ ] Create a simple C# API
// [ ] Comment code, debug warnings and such
// [ ] Demo + Documentation

// Further development
// [ ] Slide splining
// [ ] Scene handles for rotations
// [ ] Fade render pass
// [ ] Timeline for animating effects

namespace Cinemaestre {
	public enum CameraEffect { SLIDE, PAN, ZOOM, FADE }

	public enum SlideType { WORLD_POS, LOCAL_OFFSET, DIR_AND_MAG }
	public enum PanDirection { HORIZONTAL, VERTICAL }

	public class CinemaestreCamera : MonoBehaviour {
		[HideInInspector] public CameraEffect cameraEffect;

		[Header("General")]
		public float duration;
		public bool autoplay; 

		[Header("Ease Function")]
		public LeanTweenType easeType;
		public bool customEase;
		public AnimationCurve easeAnimationCurve; // only show this if customEase is true

		[Header("Looping")]
		public bool loop;
		public int loops; // this should not show up if loop is checked, and loopForever is not checked
		public bool loopForever; // should only show up if loop is checked
		public bool pingpong; // this should only show up if loop is checked

		[Header("Slide")]
		public SlideType slideType;
		public Vector3 slideWorldPos; // only show if worldpos
		public Vector3 slideLocalOffset; // only show if local offset
		public Vector3 slideMoveDir; // only show if movedir
		public float slideMoveDistance;

		[Header("Pan")]
		public PanDirection panDirection;
		public bool panCustomDirection;
		public Vector3 panAxisOfRotation; // only show this if custom direction is true
		public bool panGlobalSpace;
		public float panAngle;

		[Header("Zoom")]
		public float zoomTargetFOV;

		[Header("Fade")]
		public Image fadePanel;
		public Color fadeColor;
		public bool fadeOut = true;

		void Start() {
			if (autoplay) {
				PlayCameraEffects();
			}
		}

		/// <summary>
		/// This will begin playing the CameraEffects
		/// </summary>
		public void PlayCameraEffects() {
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
					lt.setLoopCount(loops);
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
		}

		void Update() {
			if (Keyboard.current.spaceKey.wasPressedThisFrame) {
				PlayCameraEffects();
			}
		}

		#region TWEEN FUNCTIONS
		LTDescr GetSlideLT() { // TODO: offer splining as well
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

		LTDescr GetFadeLT() { // TODO: make this into an HDRP render pass
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

