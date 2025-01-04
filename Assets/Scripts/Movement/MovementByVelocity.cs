using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour {
    private Rigidbody2D rigitBody2D;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake() {
        rigitBody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable() {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable() {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1,
        MovementByVelocityEventArgs arg2) {
        MoveRigidBody(arg2.moveDirection, arg2.moveSpeed);
    }

    private void MoveRigidBody(Vector2 arg2MoveDirection, float arg2MoveSpeed) {
        rigitBody2D.linearVelocity = arg2MoveDirection * arg2MoveSpeed;
    }
}
