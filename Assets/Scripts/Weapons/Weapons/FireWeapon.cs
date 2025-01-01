using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour {
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;

    private void Awake() {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }

    private void OnEnable() {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable() {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update() {
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent arg1, FireWeaponEventArgs arg2) {
        WeaponFire(arg2);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs) {
        WeaponPreCharge(fireWeaponEventArgs);

        if (fireWeaponEventArgs.fire) {
            if (IsWeaponReadyToFire()) {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle,
                    fireWeaponEventArgs.weaponAimDirectionVector);
                ResetCoolDownTimer();
                ResetPreChargeTimmer();
            }
        }
    }

    private void ResetPreChargeTimmer() {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs) {
        if (fireWeaponEventArgs.firePreviousFrame) {
            firePreChargeTimer -= Time.deltaTime;
        } else {
            ResetPreChargeTimmer();
        }
    }

    private void ResetCoolDownTimer() {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector) {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();
        if (currentAmmo) {
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon
                .GetShootPosition(), Quaternion.identity);

            ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector,
                activeWeapon.GetCurrentWeapon());

            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity) {
                activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
                activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
            }

            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        }
    }

    private bool IsWeaponReadyToFire() {
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon()
                .weaponDetails.hasInfiniteAmmo) {
            return false;
        }

        if (activeWeapon.GetCurrentWeapon().isWeaponReloading) {
            return false;
        }

        if (fireRateCoolDownTimer > 0 || firePreChargeTimer > 0) {
            return false;
        }

        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon()
                .weaponClipRemainingAmmo <= 0) {
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }

        return true;
    }
}
