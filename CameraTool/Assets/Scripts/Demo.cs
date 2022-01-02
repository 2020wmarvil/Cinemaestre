using UnityEngine;
using Cinemaestre;
using UnityEngine.InputSystem;

public class Demo : MonoBehaviour {
	CinemaestreCamera cam;

	void Start() {
		cam = GameObject.Find("Main Camera").GetComponent<CinemaestreCamera>();
	}

	void Update() {
		if (Keyboard.current.spaceKey.wasPressedThisFrame) {
			cam.PlayEffectStack(0);
		}
	}

	public void OnStart() {
		print("Start");
	}

	public void OnLoop() {
		print("Loop");
	}

	public void OnComplete() {
		print("Complete");
	}
}
