using UnityEngine;
using System.Collections;

public class LandingDetection : MonoBehaviour {
	public bool hasLanded = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Reset() {
		hasLanded = false;
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		// Debug.Log(collision.gameObject.name);
		if(!collision.gameObject.GetComponent<JumpTrigger>()) {
			hasLanded = true;
		}
	}
	public void OnCollisionStay2D(Collision2D collision) {
	}
	public void OnCollisionExit2D(Collision2D collision) {
	}
}
