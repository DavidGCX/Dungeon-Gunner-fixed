using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// TODO: ALL the event should be simplified to using Unity Input System later on in the project
public class PlayerControl : MonoBehaviour {
    [SerializeField] private MovementDetailsSO movementDetails;

    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake() {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start() {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Update() {
        if (isPlayerRolling) return;
        MovementInput();
        WeaponInput();
        PlayerRollCoolDownTimer();
    }

    private void PlayerRollCoolDownTimer() {
        if (playerRollCooldownTimer > 0) {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void WeaponInput() {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees,
        out AimDirection playerAimDirection) {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);
        // Prevent strange aim direction problem
        if (weaponDirection.magnitude < 2.5f) {
            weaponDirection = transform.position + (mouseWorldPosition - transform.position).normalized * 2.5f -
                              weaponShootPosition.position;
        }

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees,
            weaponDirection);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D other) {
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine() {
        if (playerRollCoroutine != null) {
            StopCoroutine(playerRollCoroutine);
            playerRollCoroutine = null;
            isPlayerRolling = false;
        }
    }

    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(weaponShootPosition.position, weaponShootPosition.right * 10f);
    //     Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
    //     Gizmos.DrawSphere(mouseWorldPosition, 0.5f);
    //     Gizmos.DrawSphere(transform.position + (mouseWorldPosition - transform.position).normalized * 2.5f, 0.5f);
    // }

    private void MovementInput() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool rightMouseDown = Input.GetMouseButtonDown(1);

        Vector2 movementDirection = new Vector2(horizontal, vertical).normalized;
        if (movementDirection != Vector2.zero) {
            if (rightMouseDown && playerRollCooldownTimer <= 0) {
                PlayerRoll((Vector3)movementDirection);
            } else {
                player.movementByVelocityEvent.CallMovementByVelocityEvent(movementDirection, moveSpeed);
            }
        } else {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void PlayerRoll(Vector3 movementDirection) {
        playerRollCoroutine = StartCoroutine(PlayerRollCoroutine(movementDirection));
    }

    private IEnumerator PlayerRollCoroutine(Vector3 movementDirection) {
        float minDistance = 0.2f;
        isPlayerRolling = true;
        Vector3 targetPosition = player.transform.position + (Vector3)movementDirection * movementDetails.rollDistance;
        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance) {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position,
                movementDetails.rollSpeed, movementDirection, isPlayerRolling);
            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;
        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif

    #endregion
}
