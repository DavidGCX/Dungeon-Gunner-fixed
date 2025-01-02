using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour {
    [Header("Object References")] [SerializeField]
    private Image weaponImage;

    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBar;
    [SerializeField] private Image barImage;
    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake() {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable() {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable() {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start() {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeapon(Weapon currentWeapon) {
        UpdateActiveWeaponImage(currentWeapon.weaponDetails);
        UpdateActiveWeaponName(currentWeapon);
        UpdateAmmoText(currentWeapon);
        UpdateAmmoLoadedIcons(currentWeapon);

        if (currentWeapon.isWeaponReloading) {
            UpdateWeaponReloadBar(currentWeapon);
        } else {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(currentWeapon);
    }

    private void UpdateActiveWeaponName(Weapon currentWeapon) {
        weaponNameText.text = "(" + currentWeapon.weaponListPosition + ") " + currentWeapon.weaponDetails.weaponName
            .ToUpper();
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO currentWeaponWeaponDetails) {
        weaponImage.sprite = currentWeaponWeaponDetails.weaponSprite;
    }

    private void WeaponReloaded(Weapon weapon) {
        if (Equals(weapon, player.activeWeapon.GetCurrentWeapon())) {
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            UpdateReloadText(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void ResetWeaponReloadBar() {
        StopReloadWeaponCoroutine();
        barImage.color = Color.green;
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void UpdateWeaponReloadBar(Weapon weapon) {
        if (weapon.weaponDetails.hasInfiniteClipCapacity) return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);
        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon) {
        barImage.color = Color.red;

        while (weapon.isWeaponReloading) {
            float barFill = weapon.weaponReloadTimer / weapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    private void StopReloadWeaponCoroutine() {
        if (reloadWeaponCoroutine != null) {
            StopCoroutine(reloadWeaponCoroutine);
            reloadWeaponCoroutine = null;
        }
    }

    private void WeaponFired(Weapon weapon) {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void UpdateAmmoText(Weapon weapon) {
        if (weapon.weaponDetails.hasInfiniteAmmo) {
            ammoRemainingText.text = "INFINITE AMMO";
        } else {
            ammoRemainingText.text = weapon.weaponRemainingAmmo + " / " + weapon.weaponDetails.weaponAmmoCapacity;
        }
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon) {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++) {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons() {
        foreach (GameObject ammoIcon in ammoIconList) {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateReloadText(Weapon weapon) {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) &&
            (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading)) {
            barImage.color = Color.red;
            StopBlinkingReloadTextCoroutine();
            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        } else {
            StopBlinkingReloadText();
        }
    }

    private void StopBlinkingReloadTextCoroutine() {
        if (blinkingReloadTextCoroutine != null) {
            StopCoroutine(blinkingReloadTextCoroutine);
            blinkingReloadTextCoroutine = null;
        }
    }

    private IEnumerator StartBlinkingReloadTextRoutine() {
        while (true) {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText() {
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent arg1, WeaponReloadedEventArgs arg2) {
        WeaponReloaded(arg2.weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2) {
        UpdateWeaponReloadBar(arg2.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent arg1, WeaponFiredEventArgs arg2) {
        WeaponFired(arg2.weapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2) {
        SetActiveWeapon(arg2.weapon);
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif

    #endregion
}
