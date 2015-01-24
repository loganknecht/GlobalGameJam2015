using FullInspector;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetPathing : MonoBehaviour {
    public GameObject gameObjectToMove = null;

    public Vector3 targetPosition = Vector3.zero;
    public List<GameObject> movementNodes = null; 

    public float xMoveSpeed = 1;
    public float yMoveSpeed = 1;

    public float targetPositionXLockBuffer = 0.05f;
    public float targetPositionYLockBuffer = 0.05f;

    public bool isPaused = false;
    public bool disableMovementLogic = false;

    public delegate void OnArrival();
    public OnArrival onArrival = null;

    protected virtual void Awake() {
        movementNodes = new List<GameObject>();
    }

    // Use this for initialization
    public virtual void Start () {
    }

    // Update is called once per frame
    public virtual void Update () {
        if(movementNodes == null) {
            Debug.Log("lol lol movement nodes of " + this.gameObject.name + " be null.");
        }
        if(!isPaused) {
            PerformLogic();
        }
    }

    public void SetXMoveSpeed(float newMoveSpeed) {
        xMoveSpeed = newMoveSpeed;
    }
    public float GetXMoveSpeed() {
        return xMoveSpeed;
    }

    public void SetYMoveSpeed(float newMoveSpeed) {
        yMoveSpeed = newMoveSpeed;
    }
    public float GetYMoveSpeed() {
        return yMoveSpeed;
    }

    public void SetMovementNodes(List<GameObject> newMovementNodes) {
        movementNodes = newMovementNodes;
    }

    public List<GameObject> GetMovementNodes() {
        return movementNodes;
    }
    public void ClearMovementNodes() {
        movementNodes.Clear();
    }

    public virtual void SetTargetPosition(List<GameObject> newMovementNodes) {
        SetMovementNodes(newMovementNodes);
        PopMovementNode();
    }
    public void SetTargetPosition(Vector3 newTargetPosition) {
        targetPosition = newTargetPosition;
    }

    public Vector3 GetTargetPosition() {
        return targetPosition;
    }

    public void SetOnArrivalAtMovementNodeLogic(OnArrival newOnArrival) {
        onArrival = new OnArrival(newOnArrival);
    }

    public bool IsAtMovementNodePosition() {
        if(gameObjectToMove.transform.position.x == targetPosition.x
            && gameObjectToMove.transform.position.y == targetPosition.y) {
            return true;
        }
        else {
            return false;
        }
    }
    public bool IsAtTargetPosition() {
        if(movementNodes.Count == 0
            && gameObjectToMove.transform.position.x == targetPosition.x
            && gameObjectToMove.transform.position.y == targetPosition.y) {
            return true;
        }
        else {
            return false;
        }
    }

    public virtual void PerformLogic() {
        if(!disableMovementLogic) {
            PerformMovementLogic();
        }
    }

    public virtual void PerformMovementLogic() {
        //This is the logic for the bro moving to their destination
        Vector2 newPositionOffset = CalculateNextPositionOffset();
        newPositionOffset = (newPositionOffset*Time.deltaTime);
        newPositionOffset = LockNewPositionOffsetToTarget(newPositionOffset);
        transform.position += new Vector3(newPositionOffset.x, newPositionOffset.y, 0);

        //performs check to pop new node from the movemeNodes list
        if(IsAtMovementNodePosition()) {
            if(onArrival != null) {
                onArrival();
            }

            //Debug.Log("object at position");
            PopMovementNode();
        }
    }

    public virtual void PopMovementNode() {
        if(movementNodes == null) {
            Debug.Log("movemeNodes is null");
        }
        if(movementNodes.Count > 0) {
            GameObject nextNode = movementNodes[0];
            targetPosition = new Vector3(nextNode.transform.position.x, nextNode.transform.position.y, this.transform.position.z);
            movementNodes.RemoveAt(0);
        }
    }

    public virtual Vector2 CalculateNextPositionOffset() {
        Vector2 newPositionOffset = Vector2.zero;

        if(gameObjectToMove.transform.position.x < targetPosition.x) {
            newPositionOffset.x += xMoveSpeed;
        }
        else if(gameObjectToMove.transform.position.x > targetPosition.x) {
            newPositionOffset.x -= xMoveSpeed;
        }

        if(gameObjectToMove.transform.position.y < targetPosition.y) {
            newPositionOffset.y += yMoveSpeed;
        }
        else if(gameObjectToMove.transform.position.y > targetPosition.y) {
            newPositionOffset.y -= yMoveSpeed;
        }

        return newPositionOffset;
    }

    public virtual Vector2 LockNewPositionOffsetToTarget(Vector2 newPositionOffset) {
        if((gameObjectToMove.transform.position.x + newPositionOffset.x) > (targetPosition.x - targetPositionXLockBuffer)
            && (gameObjectToMove.transform.position.x + newPositionOffset.x) < (targetPosition.x + targetPositionXLockBuffer)) {
            gameObjectToMove.transform.position = new Vector3(targetPosition.x,
                                                             gameObjectToMove.transform.position.y,
                                                             gameObjectToMove.transform.position.z);
            newPositionOffset.x = 0;
        }
        if((gameObjectToMove.transform.position.y + newPositionOffset.y) > (targetPosition.y - targetPositionYLockBuffer)
            && (gameObjectToMove.transform.position.y + newPositionOffset.y) < (targetPosition.y + targetPositionYLockBuffer)) {
            gameObjectToMove.transform.position = new Vector3(gameObjectToMove.transform.position.x,
                                                             targetPosition.y,
                                                             gameObjectToMove.transform.position.z);
            newPositionOffset.y = 0;
        }

        return newPositionOffset;
    }
}
