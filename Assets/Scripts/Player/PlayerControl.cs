using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// TODO: ALL the event should be simplified to using Unity Input System later on in the project
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour {
    [SerializeField] private MovementDetailsSO movementDetails;

    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 0;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake() {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    public void SetMoveSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
        SetPlayerAnimationSpeed();
    }

    private void Start() {
        waitForFixedUpdate = new WaitForFixedUpdate();
        SetStartingWeapon();
        SetPlayerAnimationSpeed();
    }


    private void SetStartingWeapon() {
        int index = 0;

        foreach (var weapon in player.weaponList) {
            if (weapon.weaponDetails == player.playerDetails.startingWeapon) {
                SetWeaponByIndex(index);
                break;
            }

            index++;
        }
    }

    private void SetWeaponByIndex(int index) {
        if (index < player.weaponList.Count) {
            currentWeaponIndex = index;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[currentWeaponIndex]);
        }
    }

    private void SetPlayerAnimationSpeed() {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }


    private void Update() {
        DebugInput();
        if (isPlayerRolling) return;
        MovementInput();
        WeaponInput();
        PlayerRollCoolDownTimer();
    }

    private void DebugInput() {
        if (Input.GetMouseButton(2)) {
            GameManager.Instance.TriggerGhostMode(true);
        }
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
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);
        SwitchWeaponInput();
        ReloadWeaponInput();
    }

    private void SwitchWeaponInput() {
        if (Input.mouseScrollDelta.y < 0f) {
            PreviousWeapon();
        }

        if (Input.mouseScrollDelta.y > 0f) {
            NextWeapon();
        }

        // Need to refactor
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetWeaponByIndex(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetWeaponByIndex(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetWeaponByIndex(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SetWeaponByIndex(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SetWeaponByIndex(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            SetWeaponByIndex(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            SetWeaponByIndex(6);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            SetWeaponByIndex(7);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            SetWeaponByIndex(8);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            SetWeaponByIndex(9);
        }

        if (Input.GetKeyDown(KeyCode.Minus)) {
            SetCurrentWeaponToFirstInTheList();
        }
    }

    private void SetCurrentWeaponToFirstInTheList() {
        List<Weapon> weaponList = new List<Weapon>();
        Weapon currentWeapon = player.weaponList[currentWeaponIndex];
        currentWeapon.weaponListPosition = 0;
        weaponList.Add(currentWeapon);
        int index = 1;

        foreach (var weapon in player.weaponList) {
            if (Equals(weapon, currentWeapon)) continue;
            weapon.weaponListPosition = index;
            weaponList.Add(weapon);
            index++;
        }

        player.weaponList = weaponList;
        currentWeaponIndex = 0;
        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon() {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0) {
            currentWeaponIndex = player.weaponList.Count - 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void NextWeapon() {
        currentWeaponIndex++;
        if (currentWeaponIndex >= player.weaponList.Count) {
            currentWeaponIndex = 0;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput() {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        if (currentWeapon.isWeaponReloading) return;

        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon
                .weaponDetails.hasInfiniteAmmo) return;
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R)) {
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees,
        AimDirection playerAimDirection) {
        if (Input.GetMouseButton(0)) {
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection,
                playerAngleDegrees, weaponAngleDegrees,
                weaponDirection);
            leftMouseDownPreviousFrame = true;
        } else {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees,
        out AimDirection playerAimDirection) {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        // Prevent strange aim direction problem
        if (weaponDirection.magnitude < Settings.useAimAngleDistance) {
            weaponDirection = playerDirection;
        }

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
