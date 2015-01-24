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
	public virtual void Start () {
		if(movementLogic == null) {
			Debug.LogError("MISSING MOVEMENT LOGIC");
		}
		// if(rigidBody == null) {
		// 	Debug.LogError("MISSING RIGIDBODY");
		// }
	}
	
	// Update is called once per frame
	public virtual void Update () {
		PerformLogic();
	}

	public void OnCollisionEnter2D() {
	}
	public void OnCollisionStay2D() {
	}
	public void OnCollisionExit2D() {
	}

	public virtual void PerformLogic() {
		PerformMovementLogic();
	}

	public virtual void PerformMovementLogic() {
	}

	public virtual void TriggerJump() {
	}
}
