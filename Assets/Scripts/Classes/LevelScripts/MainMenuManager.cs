using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public string levelToChangeTo = "";
	public GameObject playGameSoundObject;

	// Use this for initialization
	public void Start () {
		SoundManager.Instance.PlayMusic(AudioType.Melody);
	}
	
	// Update is called once per frame
	public void Update () {
		if(playGameSoundObject != null
			&& playGameSoundObject.GetComponent<AudioSource>() != null
			&& playGameSoundObject.GetComponent<AudioSource>().isPlaying == false) {
			Application.LoadLevel(levelToChangeTo);
		}
	}

	public void PerformLevelChange() {
		playGameSoundObject = SoundManager.Instance.Play(AudioType.Correct2);
	}

}
