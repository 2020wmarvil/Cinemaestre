using UnityEngine;

public class CinemaestreCamera : MonoBehaviour {
	Vector3 pos;

	private void Awake() {
		pos = transform.position;
	}

	private void Update() {
		transform.position = pos + new Vector3(0f, Mathf.Sin(Time.time), 0f);
	}
}
