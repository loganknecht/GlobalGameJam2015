using UnityEngine;
using System.Collections;

public class JumpTrigger : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		JumpDetection jumpDetectionReference = collider.gameObject.GetComponent<JumpDetection>();
		if(jumpDetectionReference != null) {
			jumpDetectionReference.detectedJump = true;
		}
	}
	public void OnTriggerStay2D(Collider2D collider) {
	}
	public void OnTriggerExit2D(Collider2D collider) {
	}
}
