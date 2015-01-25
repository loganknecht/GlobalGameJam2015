using UnityEngine;
using System.Collections;

public class TimedFadeManager : MonoBehaviour {
	//This should be added as a component to the game object this script is attached to
	public GUITexture guiTextureReference = null;

	//Defaults to black with a clear alpha
	public Color startFadeColor = new Color(0, 0, 0, 0);
	public Color targetFadeColor = new Color(0, 0, 0, 0);

	public float fadeTimer = 0f;
	public float fadeDuration = 3f;

	// Use this for initialization
	void Start () {
		if(this.gameObject.GetComponent<GUITexture>() != null) {
			guiTextureReference = this.gameObject.GetComponent<GUITexture>();
			guiTextureReference.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		}
	}
	
	// Update is called once per frame
	void Update () {
		PerformFadeLogic();
	}

	public void PerformFadeLogic() {
		if(!IsCurrentColorEqualToTargetColor()) {
			fadeTimer += Time.deltaTime;
			float currentPercentageToTargetColor = fadeTimer/fadeDuration;
			
			if(currentPercentageToTargetColor < 1) {
				Color newColor = Color.Lerp(startFadeColor, targetFadeColor, currentPercentageToTargetColor);
				guiTextureReference.color = newColor;
			}
			else {
				guiTextureReference.color = targetFadeColor;
			}
		}
	}

	public void FadeFromColorToColor(Color colorToStartFrom, Color colorToFadeTo) {
		guiTextureReference.color = colorToStartFrom;
		FadeToColor(colorToFadeTo);
	}

	public void FadeToColor(Color colorToFadeTo) {
		startFadeColor = guiTextureReference.color;
		targetFadeColor = colorToFadeTo;
		fadeTimer = 0f;
	}

	public bool IsCurrentColorEqualToTargetColor() {
		if(guiTextureReference.color.a == targetFadeColor.a
		   && guiTextureReference.color.b == targetFadeColor.b
		   && guiTextureReference.color.g == targetFadeColor.g
		   && guiTextureReference.color.r == targetFadeColor.r) {
			return true;
		}
		else {
			return false;
		}
	}
}