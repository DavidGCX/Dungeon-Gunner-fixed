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
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector) {
        int ammoCounter = 0;
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        float ammoSpawnInterval = ammoPerShot > 1
            ? Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax)
            : 0;
        while (ammoCounter < ammoPerShot) {
            ammoCounter++;
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon
                .GetShootPosition(), Quaternion.identity);

            ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector,
                activeWeapon.GetCurrentWeapon());
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity) {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        WeaponShootEffect(aimAngle);
        WeaponSoundEffect();
    }

    private void WeaponShootEffect(float aimAngle) {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect && activeWeapon.GetCurrentWeapon()
                .weaponDetails.weaponShootEffect.weaponShootEffectPrefab) {
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon
                    .GetCurrentWeapon()
                    .weaponDetails.weaponShootEffect.weaponShootEffectPrefab, activeWeapon.GetShootEffectPosition(),
                Quaternion.identity);
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect,
                aimAngle);
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    private void WeaponSoundEffect() {
        // GameManager.Instance.messageStack.AddMessage("Weapon fired", MessageType.Normal);
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect) {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails
                .weaponFiringSoundEffect);
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
