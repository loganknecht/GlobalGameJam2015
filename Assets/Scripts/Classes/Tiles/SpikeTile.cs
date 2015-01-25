using UnityEngine;
using System.Collections;

 public class SpikeTile : RunnerTile
 {
    protected override void OnActivation() {
        GameObject spikes = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tiles/SpikesDeux") as GameObject);
        Debug.Log ("spikolicious made " + spikes);
        spikes.transform.position = this.transform.position + new Vector3 (0, 1, 0);
    }
}

