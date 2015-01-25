using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueue : MonoBehaviour {
	public GameObject gameObjectToMoveWith;

	float width = 0;
	float height = 0;
	float startXPosition = 0;
	float startYPosition = 0;
	float centerX = 0;
	// float centerY = 0;
	int numberOfPatternsToDisplay = 0;
	float patternSpacing = 0;

	public List<PatternQueueObject> patternQueueObjects;

	public void Awake() {
	}

	public void Start() {
		CalculatePatternQueueLayout();
	}

	public void Update() {
		CalculatePatternQueueLayout();
	}

	public void CalculatePatternQueueLayout() {
		Vector3 cameraBottomLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(0,
																									0,
																									Camera.main.camera.nearClipPlane));

		Vector3 cameraTopRightWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(1,
																								  1,
																								  Camera.main.camera.nearClipPlane));

		width = (cameraTopRightWorldPosition.x - cameraBottomLeftWorldPosition.x);
		height = (cameraTopRightWorldPosition.y - cameraBottomLeftWorldPosition.y);

		centerX = width/2;
		// centerY = height/2;

		// This sets the start x position for the pattern queue to be offset from the left of the screen, calculating it as a slice of the total width
		float leftBound = 0 - width/2;
		float topBound = 0 + height/2;
		startXPosition = leftBound + (width/8)/2;
		startYPosition = topBound;

		Debug.Log("Start X: " + startXPosition);
		Debug.Log("Start Y: " + startYPosition);
		Debug.Log("Width: " + width);
		Debug.Log("Height: " + height);

		patternSpacing = height/numberOfPatternsToDisplay;

		patternQueueObjects = new List<PatternQueueObject>();
	}

	public void AddAtFront(Pattern patternToAdd) {
		GameObject newPatternQueueObject = PatternFactory.CreatePattern(patternToAdd);

		newPatternQueueObject.transform.parent = this.gameObject.transform;
		newPatternQueueObject.transform.localPosition = new Vector3(startXPosition, 
																startYPosition, 
																newPatternQueueObject.transform.position.z);

		PatternQueueObject patternQueueObjectReference = newPatternQueueObject.GetComponent<PatternQueueObject>();
		patternQueueObjectReference.GetTargetPathingReference().SetTargetPosition(new Vector3(startXPosition, 0, 0));
	}

	public void AddAtBack(Pattern patternToAdd) {
	}
} 












