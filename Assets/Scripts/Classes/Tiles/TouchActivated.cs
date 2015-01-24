using UnityEngine;
using System.Collections;

public class TouchActivated : MonoBehaviour {

	public bool HasBeenActivated { get; private set; }

	public void Reset() {
		HasBeenActivated = false;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		Debug.Log ("Mouse down received");
		HasBeenActivated = true;
	}
}
