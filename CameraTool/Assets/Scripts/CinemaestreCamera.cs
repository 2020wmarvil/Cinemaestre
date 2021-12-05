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
// ability to interact from script

namespace CinemaestreCamera {
	public enum CameraEffect { PAN, ZOOM, FADE, FOV }

	public class CinemaestreCamera : MonoBehaviour {
		[SerializeField] public CameraEffect cameraEffect;
		[SerializeField] bool autoplay;

		[SerializeField] bool loop;
		[SerializeField] bool pingpong;


		[SerializeField] LeanTweenType easeType;


		//[SerializeField] AnimationCurve animationCurve;

		void Update() {
			if (Keyboard.current.spaceKey.wasPressedThisFrame) {
				LeanTween.value(0f, 1f, 1f)
				 .setOnUpdate((float value) => {
				 }).setLoopPingPong()
				 .setEaseInOutCubic();
			}

		}

		void Pan() {

		}

		void Zoom() {

		}

		void Fade() { // TODO: get clarification on this one

		}

		void FOV() {

		}
	}
}

