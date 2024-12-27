using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementToPositionEvent : MonoBehaviour {
    public event System.Action<MovementToPositionEvent, MovementToPositionEventArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(Vector3 movePosition, Vector3 currentPosition, float moveSpeed,
        Vector2 moveDirection, bool isRolling) {
        OnMovementToPosition?.Invoke(this, new MovementToPositionEventArgs {
            movePosition = movePosition,
            currentPosition = currentPosition,
            moveSpeed = moveSpeed,
            moveDirection = moveDirection,
            isRolling = isRolling
        });
    }

    public class MovementToPositionEventArgs : EventArgs {
        public Vector3 movePosition;
        public Vector3 currentPosition;
        public float moveSpeed;
        public Vector2 moveDirection;
        public bool isRolling;
    }
}
