using UnityEngine;
using System.Collections;

public class RunnerTile : Tile {

  public bool HasBeenActivated { get; set; }

	public TouchActivated touchActivated;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		if (this.touchActivated.HasBeenActivated) {
			OnActivation();
			this.touchActivated.Reset();
		}
	}

  	protected virtual void OnActivation() {
		Debug.Log ("Mousetastic");
  	}
}
