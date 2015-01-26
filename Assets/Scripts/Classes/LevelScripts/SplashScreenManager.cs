using UnityEngine;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {
	public float splashScreenTimer = 0f;
	public float splashScreenDuration = 3f;
	public bool levelChangeTriggered = false;

	// Use this for initialization
	public void Start () {
		FadeManager.Instance.PerformFade(Color.black, Color.clear, 2, true);
		SoundManager.Instance.Play(AudioType.Correct1);
				// soundDictionary [AudioType.Correct1] = "Sound/success1";

	}
	
	// Update is called once per frame
	public void Update () {
		splashScreenTimer += Time.deltaTime;
		if(splashScreenTimer > splashScreenDuration
			&& !levelChangeTriggered) {
			PerformLevelChange();
			levelChangeTriggered = true;
		}
	}

	public void PerformLevelChange() {
		FadeManager.Instance.SetFadeFinishLogic(TriggerSceneChange);
		FadeManager.Instance.PerformFade(Color.clear, Color.black, 1, true);
	}

	void TriggerSceneChange() {
		Application.LoadLevel("MainMenu");
	}
}
