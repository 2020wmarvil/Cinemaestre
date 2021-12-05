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
	public class CinemaestreCamera : MonoBehaviour {
		Vector3 pos;
	
		private void Awake() {
			pos = transform.position;
		}
	
		private void Update() {
			if (Keyboard.current.spaceKey.wasPressedThisFrame) {
				transform.position = pos + new Vector3(0f, Mathf.Sin(Time.time), 0f);
	
				LeanTween.value(0f, 1f, 1f)
				 .setOnUpdate((float value) => {
					 transform.position = pos + new Vector3(0f, value, 0f);
				 }).setLoopPingPong()
				 .setEaseInOutCubic();
			}
		}
	}
}

