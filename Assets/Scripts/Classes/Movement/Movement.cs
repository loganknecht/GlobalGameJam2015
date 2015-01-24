﻿using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public float xMoveSpeed = 5f;
	public float yMoveSpeed = 5f;

	public bool movingRight = false;
	public bool movingLeft = false;

	public bool jumpTriggered = false;
	public bool hasUsedJump = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		PerformDebugKeys();
	}

	public void ResetJump() {
		jumpTriggered = false;
		hasUsedJump = false;
	}

	public Vector2 GetCurrentVelocity() {
		Vector2 velocity = Vector2.zero;
		if(movingLeft || movingRight) {
			velocity.x += (Input.GetAxis("Horizontal") * xMoveSpeed);
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
		if(Input.GetKey(KeyCode.A)) {
			movingLeft = true;
		}
		else {
			movingLeft = false;
		}
		// Moving Right Debug
		if(Input.GetKey(KeyCode.D)) {
			movingRight = true;
		}
		else {
			movingRight = false;
		}

		if(Input.GetKeyDown(KeyCode.Space)) {
			// isJumping = true;
			TriggerJump();
		}
	}

	public void TriggerJump() {
		if(!jumpTriggered
			&& !hasUsedJump) {
			jumpTriggered = true;
		}
	}
}