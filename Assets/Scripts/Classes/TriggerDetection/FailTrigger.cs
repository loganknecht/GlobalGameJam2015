using UnityEngine;
using System.Collections;

public class FailTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bot>()) {
            collision.gameObject.GetComponent<Bot>().continuouslyMoveRight = false;
            LevelManager.Instance.TriggerLevelFailedPanel();
        }
    }
    public void OnCollisionStay2D(Collision2D collision) {
    }
    public void OnCollisionExit2D(Collision2D collision) {
    }
}
