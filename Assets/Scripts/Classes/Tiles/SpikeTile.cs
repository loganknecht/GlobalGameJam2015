using UnityEngine;
using System.Collections;

 public class SpikeTile : RunnerTile
 {
 	protected override void OnActivation() {
 		Debug.Log ("Spikolicious");

    	GameObject spikes = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tiles/Spikes") as GameObject);
		spikes.transform.position = this.transform.position + new Vector3 (0, 1, 0);
	
	}
}

