using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour {
    [SerializeField] private MovementDetailsSO movementDetails;

    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private int currentWeaponIndex = 0;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    #region global Input Value
    public bool fireHold = false;
    public bool fireLastFrame = false;
    public bool rollingInput = false;
    public Vector2 movementInput = Vector2.zero;
    public Vector3 weaponDirection;
    public float weaponAngleDegrees, playerAngleDegrees;
    public AimDirection playerAimDirection;
    #endregion

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
        if (isPlayerRolling) return;
        MovementInput();
        WeaponInput();
        PlayerRollCoolDownTimer();
    }

    public void OnDebugInput(InputAction.CallbackContext value) {
        if (value.performed) {
            GameManager.Instance.TriggerGhostMode(true);
        }
    }

    private void PlayerRollCoolDownTimer() {
        if (playerRollCooldownTimer > 0) {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    #region Input
    public void OnSwitchWeaponKeyboard(InputAction.CallbackContext value) {
        string keyBoardInput = value.control.ToString().Split("/")[2];
        int index = int.Parse(keyBoardInput) - 1;
        SetWeaponByIndex(index == -1 ? 9 : index);
    }
    public void OnSetCurrentWeaponToFirstInTheList(InputAction.CallbackContext value) {
        if (value.performed) {
            SetCurrentWeaponToFirstInTheList();
        }
    }
    public void OnSwitchWeaponMouseWheel(InputAction.CallbackContext value) {
        if (!value.performed) return;

        if (value.ReadValue<Vector2>().y < 0) {
            PreviousWeapon();
        }

        if (value.ReadValue<Vector2>().y > 0) {
            NextWeapon();
        }
    }
    public void OnReloadWeapon(InputAction.CallbackContext value) {
        if (value.performed) {
            ReloadWeaponInput();
        }
    }

    public void OnMove(InputAction.CallbackContext value) {
        movementInput = value.ReadValue<Vector2>();
    }
    public void OnRoll(InputAction.CallbackContext value) {
        rollingInput = value.performed;
    }

    public void OnAim(InputAction.CallbackContext value) {
        if (value.performed) {
            Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition((Vector3)value.ReadValue<Vector2>());
            weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

            Vector3 playerDirection = (mouseWorldPosition - transform.position);

            // Prevent strange aim direction problem
            if (weaponDirection.magnitude < Settings.useAimAngleDistance) {
                weaponDirection = playerDirection;
            }

            weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
            playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
            playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
        }
    }

    public void OnFire(InputAction.CallbackContext value) {
        fireHold = value.action.IsPressed();
    }

    #endregion


    private void WeaponInput() {
        //AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
        // FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);
        // SwitchWeaponInput();
        // ReloadWeaponInput();

        // To make sure the Aim args are set in Animator
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees,
            weaponDirection);
        if (!fireHold) {
            fireLastFrame = false;
            return;
        }
        player.fireWeaponEvent.CallFireWeaponEvent(fireHold, fireLastFrame, playerAimDirection,
            playerAngleDegrees, weaponAngleDegrees,
            weaponDirection);
        fireLastFrame = true;

    }

    private void SwitchWeaponInput() {
        if (Input.mouseScrollDelta.y < 0f) {
            PreviousWeapon();
        }

        if (Input.mouseScrollDelta.y > 0f) {
            NextWeapon();
        }

        // // Need to refactor
        // if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //     SetWeaponByIndex(0);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //     SetWeaponByIndex(1);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //     SetWeaponByIndex(2);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha4)) {
        //     SetWeaponByIndex(3);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha5)) {
        //     SetWeaponByIndex(4);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha6)) {
        //     SetWeaponByIndex(5);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha7)) {
        //     SetWeaponByIndex(6);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha8)) {
        //     SetWeaponByIndex(7);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha9)) {
        //     SetWeaponByIndex(8);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha0)) {
        //     SetWeaponByIndex(9);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Minus)) {
        //     SetCurrentWeaponToFirstInTheList();
        // }
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
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees,
        AimDirection playerAimDirection) {
        if (fireHold) {
            player.fireWeaponEvent.CallFireWeaponEvent(true, fireHold, playerAimDirection,
                playerAngleDegrees, weaponAngleDegrees,
                weaponDirection);
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
            player.health.SetImmuneToDamage(false);
            player.idleEvent.CallIdleEvent();
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
        if (movementInput != Vector2.zero) {
            if (rollingInput && playerRollCooldownTimer <= 0) {
                PlayerRoll((Vector3)movementInput);
            } else {
                player.movementByVelocityEvent.CallMovementByVelocityEvent(movementInput, moveSpeed);
            }
        } else {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void PlayerRoll(Vector3 movementDirection) {
        playerRollCoroutine = StartCoroutine(PlayerRollCoroutine(movementDirection));
    }

    private IEnumerator PlayerRollCoroutine(Vector3 movementDirection) {
        float minDistance = 0.3f;
        isPlayerRolling = true;
        player.health.SetImmuneToDamage(true);
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
