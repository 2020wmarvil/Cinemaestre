using UnityEngine;
using Cinemaestre;
using UnityEngine.InputSystem;

public class Demo : MonoBehaviour {
	CinemaestreCamera cam;

	void Start() {
		cam = GameObject.Find("Main Camera").GetComponent<CinemaestreCamera>();


		// create stack, set parameters, and add it
	}

	void Update() {
		if (Keyboard.current.spaceKey.wasPressedThisFrame) {
			cam.PlayEffectStack(0);
		}
	}
}
