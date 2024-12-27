using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour {
    [SerializeField] private Transform weaponRotationPointTransform;
    private AimWeaponEvent aimWeaponEvent;

    private void Awake() {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable() {
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable() {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimEvent, AimWeaponEventArgs aimWeaponEventArgs) {
        Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.weaponAimAngle);
    }

    private void Aim(AimDirection aimDirection, float aimAngle) {
        weaponRotationPointTransform.eulerAngles = new Vector3(0, 0, aimAngle);
        switch (aimDirection) {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                break;
            case AimDirection.Right:
            case AimDirection.Up:
            case AimDirection.UpRight:
            case AimDirection.Down:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                break;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this,
            nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif

    #endregion
}
