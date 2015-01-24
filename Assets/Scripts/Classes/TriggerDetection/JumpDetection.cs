using UnityEngine;
using System.Collections;

public class JumpDetection : MonoBehaviour {
	public bool detectedJump = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Reset() {
		detectedJump = false;
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if(collision.gameObject.GetComponent<JumpTrigger>()) {
			detectedJump = true;
		}
	}
	public void OnCollisionStay2D(Collision2D collision) {
	}
	public void OnCollisionExit2D(Collision2D collision) {
	}
}
