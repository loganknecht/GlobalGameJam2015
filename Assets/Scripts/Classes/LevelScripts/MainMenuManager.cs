using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public string levelToChangeTo = "";

	// Use this for initialization
	public void Start () {
	}
	
	// Update is called once per frame
	public void Update () {
	}

	public void PerformLevelChange() {
		TriggerSceneChange();
	}

	void TriggerSceneChange() {
		Application.LoadLevel(levelToChangeTo);
	}
}
