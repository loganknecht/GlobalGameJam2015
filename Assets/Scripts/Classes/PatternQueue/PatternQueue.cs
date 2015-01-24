using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueue : MonoBehaviour {
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
	}

	public void CalculatePatternQueueLayout() {
		Vector3 cameraTopLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(0,
																								 1,
																								 Camera.main.camera.nearClipPlane));

		Vector3 cameraBottomRightWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(1,
																									 0,
																									 Camera.main.camera.nearClipPlane));

		width = (cameraBottomRightWorldPosition.x - cameraTopLeftWorldPosition.x);
		width = width/10; //This divides the actual size into a smaller width so it is a smaller slice of the screen
		height = (cameraTopLeftWorldPosition.y - cameraBottomRightWorldPosition.y);

		centerX = width/2;
		// centerY = height/2;

		startXPosition = cameraTopLeftWorldPosition.x;
		startYPosition = cameraTopLeftWorldPosition.y;

		Debug.Log("Start X: " + startXPosition + width/2);
		Debug.Log("Start Y: " + startYPosition);
		Debug.Log("Width: " + width);
		Debug.Log("Height: " + height);

		patternSpacing = height/numberOfPatternsToDisplay;

		patternQueueObjects = new List<PatternQueueObject>();
	}

	public void AddAtFront(Pattern patternToAdd) {
		GameObject newPatternQueueObject = PatternFactory.CreatePattern(patternToAdd);
		newPatternQueueObject.transform.parent = this.gameObject.transform;
		newPatternQueueObject.transform.position = new Vector3(centerX, 
																startYPosition, 
																newPatternQueueObject.transform.position.z);
		PatternQueueObject patternQueueObjectReference = newPatternQueueObject.GetComponent<PatternQueueObject>();
		patternQueueObjectReference.GetTargetPathingReference().SetTargetPosition(new Vector3(centerX, 0, 0));
	}

	public void AddAtBack(Pattern patternToAdd) {
	}
} 