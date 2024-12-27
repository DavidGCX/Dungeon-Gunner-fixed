using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour {
    private Rigidbody2D rigitBody2D;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake() {
        rigitBody2D = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable() {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable() {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1,
        MovementToPositionEvent.MovementToPositionEventArgs arg2) {
        MoveRigidBody(arg2.movePosition, arg2.currentPosition, arg2.moveSpeed);
    }

    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed) {
        Vector2 unitVector = (movePosition - currentPosition).normalized;
        rigitBody2D.MovePosition(rigitBody2D.position + unitVector * moveSpeed * Time.fixedDeltaTime);
    }
}
