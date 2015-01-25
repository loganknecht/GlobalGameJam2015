using FullInspector;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueObject : BaseBehavior {
    public TargetPathing targetPathing;
    public List<Pattern> patterns;
    public float timer = 0f;
    public float timerMax = 5f;
    public bool isPaused = false;

    protected override void Awake() {
        if(patterns == null) {
            patterns = new List<Pattern>();
        }
    }

    public void Start() {
    }

    public void Update() {
        timer += Time.deltaTime;
    }

    public bool HasExpired() {
        if(timer > timerMax) {
            return true;
        }
        else {
            return false;
        }

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

    public void DestroyOnArrival() {
        Destroy(this.gameObject);
    }
} 