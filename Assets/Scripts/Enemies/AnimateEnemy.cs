using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour {
    private Enemy enemy;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable() {
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable() {
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1,
        MovementToPositionEvent.MovementToPositionEventArgs arg2) {
        if (enemy.transform.position.x < GameManager.Instance.GetPlayer().transform.position.x) {
            SetAimWeaponAnimationParameters(AimDirection.Right);
        } else {
            SetAimWeaponAnimationParameters(AimDirection.Left);
        }

        SetMovementAnimationParameters();
    }

    private void SetMovementAnimationParameters() {
        enemy.animator.SetBool(Settings.isMoving, true);
        enemy.animator.SetBool(Settings.isIdle, false);
    }

    private void SetAimWeaponAnimationParameters(AimDirection aimDirection) {
        InitializeAimAnimationParameters();
        switch (aimDirection) {
            case AimDirection.Up:
                enemy.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpRight:
                enemy.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.Left:
                enemy.animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.UpLeft:
                enemy.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Right:
                enemy.animator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.Down:
                enemy.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }

    private void InitializeAimAnimationParameters() {
        enemy.animator.SetBool(Settings.aimDown, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
    }

    private void IdleEvent_OnIdle(IdleEvent obj) {
        SetIdleAnimationParameters();
    }

    private void SetIdleAnimationParameters() {
        enemy.animator.SetBool(Settings.isMoving, true);
        enemy.animator.SetBool(Settings.isIdle, false);
    }
}
