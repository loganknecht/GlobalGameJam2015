using FullInspector;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueObject : BaseBehavior {
    public TargetPathing targetPathing;
    public List<Pattern> patterns;
    public Animator animationController;
    public float timer = 0f;
    public float timerMax = 5f;
    public bool isPaused = false;

    protected override void Awake() {
        if(patterns == null) {
            patterns = new List<Pattern>();
        }
    }

    public void Start() {
        Pattern randomPattern = GetRandomPattern();
        while(randomPattern == Pattern.None) {
            randomPattern = GetRandomPattern();
        }
        patterns.Add(randomPattern);
    }

    public void Update() {
        UpdateAnimationController();
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

    public Pattern GetRandomPattern() {
        Pattern[] patterns = (Pattern[])Pattern.GetValues(typeof(Pattern));
        return patterns[Random.Range(0, patterns.Length)];
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

    public void UpdateAnimationController() {
        foreach(Pattern pattern in Pattern.GetValues(typeof(Pattern))) {
            animationController.SetBool(pattern.ToString(), false);
        }
        foreach(Pattern pattern in patterns) {
            animationController.SetBool(pattern.ToString(), true);
        }
    }
} 