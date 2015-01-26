using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
    public float xMoveSpeed = 5f;
    public float yMoveSpeed = 5f;

    public bool movingRight = false;
    public bool movingLeft = false;

    public bool jumpTriggered = false;
    public bool hasUsedJump = false;
    public bool debug = false;

    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
        if(debug) {
            PerformDebugKeys();
        }
    }

    public Vector2 GetCurrentVelocity() {
        Vector2 velocity = Vector2.zero;

        if(movingLeft) {
            // velocity.x = (Input.GetAxis("Horizontal") * xMoveSpeed);
            velocity.x = (-1 * xMoveSpeed);
        }
        if(movingRight) {
            // velocity.x = (Input.GetAxis("Horizontal") * xMoveSpeed);
            velocity.x = (1 * xMoveSpeed);
        }

        if(jumpTriggered
            && !hasUsedJump) {
            velocity.y = yMoveSpeed;
            hasUsedJump = true;
        }

        return velocity;
    }

    public Vector2 CalculateMovementStep() {
        return GetCurrentVelocity();
    }

    public void PerformDebugKeys() {
        // Moving Left Debug
        // if(Input.GetKey(KeyCode.A)) {
        //     movingLeft = true;
        // }
        // else {
        //     movingLeft = false;
        // }
        // // Moving Right Debug
        // if(Input.GetKey(KeyCode.D)) {
        //     movingRight = true;
        // }
        // else {
        //     movingRight = false;
        // }

        if(Input.GetKeyDown(KeyCode.Space)) {
            TriggerJump();
            if(!hasUsedJump) {
                SoundManager.Instance.Play(Random.Range(0, 100) > 50 ? AudioType.Jump2 : AudioType.Jump1);
            }
        }
    }

    public void TriggerJump() {
        if(!jumpTriggered
            && !hasUsedJump) {
            jumpTriggered = true;
        }
    }

    public void ResetJump() {
        jumpTriggered = false;
        hasUsedJump = false;
    }
}
