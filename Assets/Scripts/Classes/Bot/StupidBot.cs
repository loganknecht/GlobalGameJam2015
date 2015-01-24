using UnityEngine;
using System.Collections;

public class StupidBot : Bot {

    // Use this for initialization
    public virtual void Start () {
        base.Start();
    }
    
    // Update is called once per frame
    public override void Update () {
        base.Update();
    }

    public void OnCollisionEnter2D(Collision2D collision) {
    }
    public void OnCollisionStay2D(Collision2D collision) {
    }
    public void OnCollisionExit2D(Collision2D collision) {
    }

    public override void PerformMovementLogic() {
        Vector2 movementStep = movementLogic.CalculateMovementStep();

        rigidBody.AddForce(new Vector2(movementStep.x, movementStep.y));
        if(!movementLogic.movingLeft
            && !movementLogic.movingRight) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }

        if(landingDetection.hasLanded) {
            movementLogic.ResetJump();
            landingDetection.Reset();
        }
    }

    public override void TriggerJump() {
    }
}
