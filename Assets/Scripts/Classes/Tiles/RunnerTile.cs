using UnityEngine;
using System.Collections;

public class RunnerTile : Tile {

    public bool HasBeenActivated {
        get;
        set;
    }

    public Pattern pattern = Pattern.None;

    public TouchActivated touchActivated;

    private PatternQueue PQ {
        get {
            return PatternQueueManager.Instance.patternQueue;
        }
    }

    public Animator animationController;

    // Use this for initialization
    void Start () {
        // var values = Pattern.GetValues(Pattern.GetType());
        Pattern[] patterns = (Pattern[])Pattern.GetValues(typeof(Pattern));
        while(pattern == Pattern.None) {
            int num = (int)(Random.value * patterns.Length);
            pattern = (Pattern)patterns.GetValue(num);   
        }
        // Debug.Log ("I chose pattern " + this.pattern);
    }

    // Update is called once per frame
    public override void Update () {
        if (this.touchActivated.HasBeenActivated) {
            if(PQ.ContainsPattern(this.pattern)) {
                SoundManager.Instance.Play(AudioType.Correct1);
                OnActivation();
            } else {
                SoundManager.Instance.Play(AudioType.Correct2);
            }
            this.touchActivated.Reset();
        }
        UpdateAnimationController();
    }

    protected virtual void OnActivation() {
        Debug.Log ("Mousetastic");
    }

    public void UpdateAnimationController() {
        foreach(Pattern pattern in Pattern.GetValues(typeof(Pattern))) {
            animationController.SetBool(pattern.ToString(), false);
        }
        animationController.SetBool(this.pattern.ToString(), true);
    }
}
