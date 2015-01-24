using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Movement))]
// [RequireComponent (typeof(Rigidbody))]
public class Bot : MonoBehaviour {

	public Movement movementLogic;
	public Rigidbody2D rigidBody;
	public LandingDetection landingDetection;
	public Vector2 maxVelocity = new Vector2(5, 5);

	// Use this for initialization
	void Start () {
		if(movementLogic == null) {
			Debug.LogError("MISSING MOVEMENT LOGIC");
		}
		// if(rigidBody == null) {
		// 	Debug.LogError("MISSING RIGIDBODY");
		// }
	}
	
	// Update is called once per frame
	void Update () {
		PerformMovementLogic();
	}

	public void OnCollisionEnter2D() {
	}
	public void OnCollisionStay2D() {
	}
	public void OnCollisionExit2D() {
	}

	public void PerformMovementLogic() {
		Vector2 movementStep = movementLogic.CalculateMovementStep();
		// movementStep *= Time.deltaTime;
		// this.gameObject.transform.position += new Vector3(movementStep.x,
		// 												  movementStep.y,
		// 												  0);
		// this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x + movementStep.x, 
		// 												 this.gameObject.transform.position.y,
		// 												 // this.gameObject.transform.position.y + movementStep.y,
		// 												 this.gameObject.transform.position.z);

		rigidBody.AddForce(new Vector2(movementStep.x, movementStep.y));
		if(!movementLogic.movingLeft
			&& !movementLogic.movingRight) {
			// rigidBody.velocity.x = 0;
			rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
		}
		// rigidBody.AddForce(new Vector3(movementStep.x, movementStep.y, 0));
		// rigidBody.velocity = new Vector3(Mathf.Clamp(rigidBody.velocity.x, -maxVelocity.x, maxVelocity.x),
		// 								 Mathf.Clamp(rigidBody.velocity.y, -maxVelocity.y, maxVelocity.y),
		// 								 0);

		if(landingDetection.hasLanded) {
			movementLogic.ResetJump();
			landingDetection.Reset();
		}
	}

	public void Jump() {
	}
}
