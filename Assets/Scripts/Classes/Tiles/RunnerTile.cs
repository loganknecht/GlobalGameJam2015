using UnityEngine;
using System.Collections;

public class RunnerTile : Tile {

    public bool HasBeenActivated {
        get;
        set;
    }

    public Pattern Pattern {
        get;
        set;
    }

    public TouchActivated touchActivated;

    private PatternQueue PQ {
        get {
            return PatternQueueManager.Instance.patternQueue;
        }
    }

    // Use this for initialization
    void Start () {
        var values = Pattern.GetValues (Pattern.GetType ());
        int num = (int)(Random.value * values.Length);
        Pattern = (Pattern)values.GetValue (num);   
        Debug.Log ("I chose pattern " + this.Pattern);
    }

    // Update is called once per frame
    public override void Update () {
        if (this.touchActivated.HasBeenActivated) {
            if (PQ.ContainsPattern(this.Pattern)) {
                SoundManager.Instance.Play(AudioType.Correct1);
                OnActivation();
            } else {
                SoundManager.Instance.Play(AudioType.Correct2);
            }
            this.touchActivated.Reset();
        }
    }

    protected virtual void OnActivation() {
        Debug.Log ("Mousetastic");
    }
}
