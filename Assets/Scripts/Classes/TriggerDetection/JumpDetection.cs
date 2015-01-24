using UnityEngine;
using System.Collections;

public class JumpDetection : MonoBehaviour {
	public bool detectedJump = true;

	public bool detectedSpikes = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ResetSpikes() {
		detectedSpikes = false;
	}

	public void Reset() {
		detectedJump = false;
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if(collision.gameObject.GetComponent<JumpTrigger>()) {
			detectedJump = true;
		}
		if (collision.gameObject.tag == "Spikes") {
			detectedSpikes = true;
		}
	}
	public void OnCollisionStay2D(Collision2D collision) {
	}
	public void OnCollisionExit2D(Collision2D collision) {
	}
}
