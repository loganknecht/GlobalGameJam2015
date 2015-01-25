using UnityEngine;
using System.Collections;

public class StupidBot : Bot {
    // Use this for initialization
    public override void Start () {
        base.Start();
    }
    
    // Update is called once per frame
    public override void Update () {
        if(continuouslyMoveRight) {
            movementLogic.movingRight = true;
        }
        base.Update();
    }

    public override void PerformMovementLogic() {
        Vector2 movementStep = movementLogic.CalculateMovementStep();

        // If moving left, but velocity is right direction, then set to zero
        if(movementLogic.movingLeft
            && !movementLogic.movingRight
            && rigidBody.velocity.x > 0) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
        // If moving right and not moving left, but velocity is left direction, then set to zero
        if(movementLogic.movingRight
            && !movementLogic.movingLeft
            && rigidBody.velocity.x < 0) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
        // resets velocity if not moving left or right so deceleration just a flat stop
        if(!movementLogic.movingLeft
            && !movementLogic.movingRight) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }

        rigidBody.AddForce(new Vector2(movementStep.x, movementStep.y));

        if(rigidBody.velocity.x > maxVelocity.x) {
            rigidBody.velocity = new Vector2(maxVelocity.x, rigidBody.velocity.y);
        }
        if(rigidBody.velocity.y > maxVelocity.y) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxVelocity.y);
        }

    }

    public override void PerformJumpLogic() {
        if(landingDetection.hasLanded) {
            // Debug.Log("Landed!");
            movementLogic.ResetJump();
            landingDetection.Reset();
        }

        if(jumpDetection.detectedJump) {
            var obj = SoundManager.Instance.Play(Random.Range(0, 100) > 50 ? AudioType.Jump2 : AudioType.Jump1);
            Debug.Log("playing sound " + obj);
            movementLogic.TriggerJump();
            jumpDetection.Reset();
        }

        if(jumpDetection.detectedSpikes) {
          jumpDetection.ResetSpikes();
          rigidBody.velocity = rigidBody.velocity * 0.5f;
          Debug.Log("spikes zomg");
        }
    }
}
