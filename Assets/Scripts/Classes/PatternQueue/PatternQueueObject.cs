using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueObject : MonoBehaviour {
    public TargetPathing targetPathing;
    public List<Pattern> patterns;
    public float timer = 0f;
    public bool isPaused = false;

    public void Awake() {
        patterns = new List<Pattern>();
    }

    public void Start() {
    }

    public void Update() {
        timer += Time.deltaTime;
    }

    public TargetPathing GetTargetPathingReference() {
        return targetPathing;
    }

    public void PerformMovementLogic() {
    }

    public void SetPatterns(params Pattern[] patternsToSet) {
        patterns.Clear();
        foreach(Pattern pattern in patternsToSet) {
            patterns.Add(pattern);
        }
    }
} 