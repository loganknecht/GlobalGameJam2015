using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// WARNING! WARNING! WARNING!
/// This code makes an assumption that it is a childe object of another component,
/// and with this assumption it configures its position with respect to its local world coordinates
/// Because of this when calculating from TOP to BOTTOM the center between those points is 0
/// This means that the top border is always calculated by 0 + height/2, and the bottom
/// is calculated by 0 - height/2. The same goes for width
/// </summary>
public class PatternQueue : MonoBehaviour {

    float width = 0;
    float height = 0;
    float objectHeight = 1f;
    float yPositionIncrement = 0;
    float borderBuffer = 0.5f;

    public int numberOfPatternsToDisplay = 1;

    public List<PatternQueueObject> patternQueueObjects;

    public void Awake() {
    }

    public void Start() {
        patternQueueObjects = new List<PatternQueueObject>();

        CalculateInitialPatternQueueLayout();
    }

    public void Update() {
    }

    public bool ContainsPattern(Pattern typeToCheck) {
        foreach(PatternQueueObject patternQueueObject in patternQueueObjects) {
            foreach(Pattern pattern in patternQueueObject.patterns) {
                if(pattern == typeToCheck) {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddPattern() {
        CalculateInitialPatternQueueLayout();

        float startXPosition = 0 - width/2;
        float startYPosition = (0 + height/2) - (objectHeight/2) - borderBuffer;

        GameObject newPatternQueueObject = PatternFactory.CreatePattern();
        newPatternQueueObject.transform.parent = this.gameObject.transform;
        newPatternQueueObject.transform.localPosition = new Vector3(startXPosition, 
                                                                    startYPosition, 
                                                                    newPatternQueueObject.transform.position.z);

        patternQueueObjects.Add(newPatternQueueObject.GetComponent<PatternQueueObject>());

        CalculateNewLayout();
    }

    public void CalculateInitialPatternQueueLayout() {
        Vector3 cameraBottomLeftWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(0,
                                                                                                    0,
                                                                                                    Camera.main.camera.nearClipPlane));

        Vector3 cameraTopRightWorldPosition = Camera.main.camera.ViewportToWorldPoint(new Vector3(1,
                                                                                                  1,
                                                                                                  Camera.main.camera.nearClipPlane));

        width = (cameraTopRightWorldPosition.x - cameraBottomLeftWorldPosition.x);
        height = (cameraTopRightWorldPosition.y - cameraBottomLeftWorldPosition.y);

        // Debug.Log("Start X: " + startXPosition);
        // Debug.Log("Start Y: " + startYPosition);
        // Debug.Log("Width: " + width);
        // Debug.Log("Height: " + height);
    }

    public void CalculateNewLayout() {
        //----------------------------------------------------------------------------------
        // This BADLY needs to be fixed, but for game jam status we're moving on
        //----------------------------------------------------------------------------------
        float startXPosition = 0 - width/2;
        float xPosition = startXPosition + (width/8)/2;

        float startYPosition = (0 + height/2) - (objectHeight/2) - borderBuffer;
        float yPosition = startYPosition;
        // Include a border buffer for the calculation
        float yPositionIncrement = (height - (borderBuffer*2))/patternQueueObjects.Count;

        for(int i = patternQueueObjects.Count - 1; i >= 0; i--) {
            PatternQueueObject currentPatternQueueObject = patternQueueObjects[i];
            currentPatternQueueObject.GetTargetPathingReference().SetTargetPosition(new Vector3(xPosition, 
                                                                                                yPosition,
                                                                                                currentPatternQueueObject.transform.position.z));
            yPosition -= yPositionIncrement;
        }
    }
} 












