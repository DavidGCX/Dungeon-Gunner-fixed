using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject {
    [Space(10)] [Header("Weapon Base Details")]
    public string weaponName;

    public Sprite weaponSprite;

    [Space(10)] [Header("Weapon Configuration")]
    public Vector3 weaponShootPosition;

    public AmmoDetailsSO weaponCurrentAmmo;
    public WeaponShootEffectSO weaponShootEffect;
    public SoundEffectSO weaponFiringSoundEffect;
    public SoundEffectSO weaponReloadingSoundEffect;

    [Space(10)] [Header("Weapon Operating Values")]
    public bool hasInfiniteAmmo = false;

    public bool hasInfiniteClipCapacity = false;

    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity = 100;
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f;
    public float weaponReloadTime = 0f;

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);
        if (!hasInfiniteAmmo) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity,
                false);
        }
    }
#endif

    #endregion
}
