using UnityEngine;
using System.Collections;

 public class SpikeTile : RunnerTile
 {
 	public GameObject spikesPrefab;

 	protected override void OnActivation() {
 		Debug.Log ("Spikolicious");

    	GameObject spikes = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Spikes") as GameObject);
		spikes.tag = "Spikes";
		spikes.transform.position = this.transform.position + new Vector3 (0, 1, 0);
	
	}
}

