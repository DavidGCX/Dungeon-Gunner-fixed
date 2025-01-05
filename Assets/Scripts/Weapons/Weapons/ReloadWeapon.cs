using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour {
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;

    private Coroutine reloadWeaponCoroutine;

    private void Awake() {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable() {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable() {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2) {
        if (reloadWeaponCoroutine != null) {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(arg2.weapon, arg2.topUpAmmoPercent));
    }

    private IEnumerator ReloadWeaponCoroutine(Weapon weapon, int topUpAmmoPercent) {
        if (weapon.weaponDetails.weaponReloadingSoundEffect) {
            SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.weaponReloadingSoundEffect);
        }

        weapon.isWeaponReloading = true;
        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime) {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        if (topUpAmmoPercent != 0) {
            int ammoIncrease = Mathf.RoundToInt(weapon.weaponDetails.weaponAmmoCapacity * topUpAmmoPercent / 100f);
            weapon.weaponRemainingAmmo = Mathf.Clamp(weapon.weaponRemainingAmmo + ammoIncrease, 0,
                weapon.weaponDetails.weaponAmmoCapacity);
        }

        if (weapon.weaponDetails.hasInfiniteAmmo) {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        } else if (weapon.weaponRemainingAmmo >= weapon.weaponDetails.weaponClipAmmoCapacity) {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        } else {
            weapon.weaponClipRemainingAmmo = weapon.weaponRemainingAmmo;
        }

        weapon.weaponReloadTimer = 0f;
        weapon.isWeaponReloading = false;
        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2) {
        if (arg2.weapon.isWeaponReloading) {
            if (reloadWeaponCoroutine != null) {
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(arg2.weapon, 0));
        }
    }

    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
