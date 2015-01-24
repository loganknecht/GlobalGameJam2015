using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueObject : MonoBehaviour {
    TargetPathing targetPathing;
    List<Pattern> patterns;

    public void Awake() {
        patterns = new List<Pattern>();
    }

    public void Start() {
    }

    public void Update() {
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