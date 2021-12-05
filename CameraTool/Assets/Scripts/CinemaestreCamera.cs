using UnityEngine;
using UnityEngine.InputSystem;

// TODO: 
// [ ] Pan (in cardinal directions)
// [ ] Zoom (in and out with an adjustable time rate)
// [ ] Fade Screen (using hdrp)
// [ ] Lerp FOV  (in and out with an adjustable time rate)
// [ ] Trigger/ Enable / Disable GameObjects or Functions

// enum to choose mode
// editor depending on that enum
// someway to trigger event
// ability to interact from script (API)
// layer and blend multiple effects
// create a timeline of effects?

namespace Cinemaestre {
	public enum CameraEffect { PAN, ZOOM, FADE, FOV }

	public class CinemaestreCamera : MonoBehaviour {
		[HideInInspector] public CameraEffect cameraEffect;

		public LeanTweenType easeType;
		public bool customEase;
		public AnimationCurve easeAnimationCurve; // only show this if customEase is true
		public bool autoplay;
		public bool loopForever;
		public int loops; // this should not show up if loop is checked
		public bool pingpong; // this should only show up if loop is checked

		public float duration;

		public float to;
		public float from;

		void Update() {
			if (Keyboard.current.spaceKey.wasPressedThisFrame) {
				Vector3 initialPos = transform.position;

				LTDescr lt = LeanTween.value(0f, 1f, duration)
					.setOnUpdate((float value) => {
						//Slide(value, initialPos);
						Pan(value, Quaternion.identity);
					}).setLoopPingPong(loops);

				if (customEase) { // make this work
					lt.setEase(easeAnimationCurve);
				} else {
					lt.setEase(easeType);
				}
			}
		}

		void Slide(float value, Vector3 initialPos) {
			transform.position = initialPos + new Vector3(0f, 0f, value);
		}

		void Pan(float value, Quaternion initialRot) {
		//	transform.position = initialPos + new Vector3(0f, 0f, value);
	        transform.rotation = Quaternion.identity * Quaternion.AngleAxis(value, Vector3.up);
		}

		void Zoom() { // find a way to do this without changing the FOV

		}

		void Fade() { // screen fade

		}

		void FOV() {

		}
	}
}

