using UnityEngine;
using System.Collections;

public class StupidBot : Bot {

    // Use this for initialization
    public override void Start () {
        base.Start();
    }
    
    // Update is called once per frame
    public override void Update () {
        base.Update();
    }

    public override void PerformMovementLogic() {
        Vector2 movementStep = movementLogic.CalculateMovementStep();

        rigidBody.AddForce(new Vector2(movementStep.x, movementStep.y));

        // resets velocity if not moving left or right so deceleration just a flat stop
        if(!movementLogic.movingLeft
            && !movementLogic.movingRight) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
    }

    public override void PerformJumpLogic() {
        if(landingDetection.hasLanded) {
            // Debug.Log("Landed!");
            movementLogic.ResetJump();
            landingDetection.Reset();
        }

        if(jumpDetection.detectedJump) {
            movementLogic.TriggerJump();
            jumpDetection.Reset();
        }
    }
}
