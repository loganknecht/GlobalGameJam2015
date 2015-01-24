using UnityEngine;
using System.Collections;

public class SpikeTile : RunnerTile
{
	public GameObject spikesPrefab;

	protected override void OnActivation() {
		Debug.Log ("Spikolicious");

		Object.Instantiate (spikesPrefab, this.transform.position + new Vector2(0, 1));
	}
}

