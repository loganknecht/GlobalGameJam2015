using UnityEngine;
using System.Collections;

public class FadeManager : MonoBehaviour {

//	GUI.color This will affect both backgrounds & text colors.
//	GUI.backgroundColor Global tinting color for all background elements rendered by the GUI.
//	GUI.contentColor Tinting color for all text rendered by the GUI.

  public delegate void FadeFinishLogic();
  public FadeFinishLogic fadeFinishLogic = null;

	//BEGINNING OF SINGLETON CODE CONFIGURATION
	private static volatile FadeManager _instance;
	private static object _lock = new object();

	//Stops the lock being created ahead of time if it's not necessary
	static FadeManager() {
	}

	public static FadeManager Instance {
		get {
			if(_instance == null) {
				lock(_lock) {
					if (_instance == null) {
						GameObject fadeManagerGameObject = new GameObject("FadeManagerGameObject");
						_instance = (fadeManagerGameObject.AddComponent<FadeManager>()).GetComponent<FadeManager>();
					}
				}
			}
			return _instance;
		}
	}

	private FadeManager() {
	}
	//END OF SINGLETON CODE CONFIGURATION

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public void SetFadeFinishLogic(FadeFinishLogic newFadeFinishLogic) {
		fadeFinishLogic = new FadeFinishLogic(newFadeFinishLogic);
	}
	public void PerformFade(Color startFadeColor, Color endFadeColor, float fadeDuration, bool destroyFullScreenTextureAtFinish) {
		StartCoroutine(PerformFullScreenFade(startFadeColor, endFadeColor, fadeDuration, destroyFullScreenTextureAtFinish));
	}

	public IEnumerator PerformFullScreenFade(Color startFadeColor, Color endFadeColor, float fadeDuration, bool destroyFullScreenTextureAtFinish) {
		Texture2D texture2D = new Texture2D(1,1);
		texture2D.SetPixel(0,0, Color.white);
		texture2D.wrapMode = TextureWrapMode.Repeat;
		texture2D.Apply();

		GameObject tempGameObj = new GameObject("Full Screen Fade GameObject");
		tempGameObj.AddComponent<GUITexture>();
		tempGameObj.GetComponent<GUITexture>().texture = texture2D;
		tempGameObj.GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	tempGameObj.transform.position = new Vector3(tempGameObj.transform.position.x, tempGameObj.transform.position.y, 0);

		float fadeTimer = 0f;
		bool fadeFinished = false;
		while(!fadeFinished) {
			fadeTimer += Time.deltaTime;

			tempGameObj.GetComponent<GUITexture>().color = PerformFadeLogic(startFadeColor, endFadeColor, fadeTimer, fadeDuration);

			if(AreTheseColorsEqual(tempGameObj.GetComponent<GUITexture>().color, endFadeColor)) {
				fadeFinished = true;
			}

			yield return new WaitForEndOfFrame();
		}

	PerformFadeFinishedLogic();

		if(destroyFullScreenTextureAtFinish) {
			Destroy(tempGameObj);
			Destroy(texture2D);
		}
	}

	public IEnumerator PerformGUIStyleFontFade(GUIStyle styleFontToFade, Color startFadeColor, Color endFadeColor, float fadeDuration) {
		float fadeTimer = 0f;
		bool fadeFinished = false;
		while(!fadeFinished) {
			fadeTimer += Time.deltaTime;

			styleFontToFade.normal.textColor = PerformFadeLogic(startFadeColor, endFadeColor, fadeTimer, fadeDuration);

			if(AreTheseColorsEqual(styleFontToFade.normal.textColor, endFadeColor)) {
				fadeFinished = true;
			}

			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	public Color PerformFadeLogic(Color startFadeColor, Color targetFadeColor, float fadeTimer, float fadeDuration) {
		if(fadeTimer < fadeDuration) {
			return Color.Lerp(startFadeColor, targetFadeColor, (fadeTimer/fadeDuration));
		}
		else {
			return targetFadeColor;
		}
	}

	public bool AreTheseColorsEqual(Color colorOne, Color colorTwo) {
		if(colorOne.a == colorTwo.a
		   && colorOne.b == colorTwo.b
		   && colorOne.g == colorTwo.g
		   && colorOne.r == colorTwo.r) {
			return true;
		}
		else {
			return false;
		}
	}

  public void PerformFadeFinishedLogic() {
	if(fadeFinishLogic != null) {
	  fadeFinishLogic();
	}
  }
}
