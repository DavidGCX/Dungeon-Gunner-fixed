using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour {
    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void OnEnable() {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable() {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1,
        MovementToPositionEvent.MovementToPositionEventArgs arg2) {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetMovementToPositionAnimationParameters(arg2);
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs) {
        InitializeAimAnimationParameters();
        // clear the roll animation parameters
        InitializeRollAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1,
        MovementByVelocityEventArgs arg2) {
        // clear the roll animation parameters
        InitializeRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    private void IdleEvent_OnIdle(IdleEvent obj) {
        // clear the roll animation parameters
        InitializeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    private void SetMovementToPositionAnimationParameters(
        MovementToPositionEvent.MovementToPositionEventArgs movementToPositionEventArgs) {
        if (movementToPositionEventArgs.isRolling) {
            if (movementToPositionEventArgs.moveDirection.x > 0) {
                player.animator.SetBool(Settings.rollRight, true);
            } else if (movementToPositionEventArgs.moveDirection.x < 0) {
                player.animator.SetBool(Settings.rollLeft, true);
            } else if (movementToPositionEventArgs.moveDirection.y > 0) {
                player.animator.SetBool(Settings.rollUp, true);
            } else if (movementToPositionEventArgs.moveDirection.y < 0) {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }

    private void SetIdleAnimationParameters() {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    private void SetMovementAnimationParameters() {
        player.animator.SetBool(Settings.isIdle, false);
        player.animator.SetBool(Settings.isMoving, true);
    }

    private void SetAimWeaponAnimationParameters(AimDirection aimDirection) {
        switch (aimDirection) {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }

    private void InitializeRollAnimationParameters() {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollUp, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollRight, false);
    }

    private void InitializeAimAnimationParameters() {
        player.animator.SetBool(Settings.aimDown, false);
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimUpRight, false);
    }
}
