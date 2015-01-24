using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueue : MonoBehaviour {
	float width = 0;
	float height = 0;
	float startXPosition = 0;
	float startYPosition = 0;
	int numberOfPatternsToDisplay = 0;
	float patternSpacing = 0;

	public List<PatternQueueObject> patternQueueObjects;

	public void Awake() {
	}

	public void Start() {
		// Debug.Log(Camera.main.camera.pixelRect);
		// Vector3 cameraTopLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(Camera.main.camera.pixelRect.x,
		// 																			 Camera.main.camera.pixelRect.y,
		// 																			 Camera.main.camera.nearClipPlane));
		Vector3 cameraTopLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(0,
																								 1,
																								 Camera.main.camera.nearClipPlane));

		//  Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 5));
		// groundBase.transform.position = worldPoint;
		// Vector3 cameraTopLeftWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.camera.nearClipPlane));

		Vector3 cameraRightWorldPosition = Camera.main.camera.ViewportToWorldPoint();
		// Vector3 cameraTopLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(0,
		// 																						 1,
		// 																						 Camera.main.camera.nearClipPlane));
		
		// width = Camera.main.camera.pixelRect.width/10;
		// height = Camera.main.camera.pixelRect.height;
		startXPosition = cameraTopLeftWorldPosition.x;
		startYPosition = cameraTopLeftWorldPosition.y;

		Debug.Log("Start X: " + startXPosition);
		Debug.Log("Start Y: " + startYPosition);
		Debug.Log("Width: " + width);
		Debug.Log("Height: " + height);

		patternSpacing = height/numberOfPatternsToDisplay;

		patternQueueObjects = new List<PatternQueueObject>();
	}

	public void Update() {
	}

	public void AddAtFront(Pattern patternToAdd) {
		GameObject newPatternQueueObject = PatternFactory.CreatePattern(patternToAdd);
		newPatternQueueObject.transform.parent = this.gameObject.transform;
		newPatternQueueObject.transform.position = new Vector3(startXPosition, 
																startYPosition, 
																newPatternQueueObject.transform.position.z);
		newPatternQueueObject.GetComponent<PatternQueueObject>();
	}

	public void AddAtBack(Pattern patternToAdd) {
	}
} 