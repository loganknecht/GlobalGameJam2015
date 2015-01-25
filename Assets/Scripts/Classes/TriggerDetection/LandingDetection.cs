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

	public void OnTriggerEnter2D(Collider2D collider) {
		// Debug.Log(collider.gameObject.name);
		if(!collider.gameObject.GetComponent<JumpTrigger>()) {
			hasLanded = true;
		}
	}
	public void OnTriggerStay2D(Collider2D collider) {
	}
	public void OnTriggerExit2D(Collider2D collider) {
	}
}
