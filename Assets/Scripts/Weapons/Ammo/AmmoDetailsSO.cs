using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject {
    [Space(10)] [Header("Ammo Base Details")]
    public string ammoName;

    public bool isPlayerAmmo;

    [Space(10)] [Header("Ammo Sprite, Prefab and Materials")]
    public Sprite ammoSprite;

    public GameObject[] ammoPrefabArray;
    public Material ammoMaterial;

    [Tooltip("Time for ammo to stay at shooting position before it is shot out")]
    public float ammoChargeTime = 0f;

    public Material ammoChargeMaterial;

    [Space(10)] [Header("Ammo Base Parameters")]
    public int ammoDamage = 1;

    public float ammoSpeedMin = 20f;

    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRotationSpeed = 1f;

    [Space(10)] [Header("Ammo Spread Details")]
    public float ammoSpreadMin = 0f;

    public float ammoSpreadMax = 0f;

    [Space(10)] [Header("Ammo Spawn Details")]
    public int ammoSpawnAmountMin = 1;

    public int ammoSpawnAmountMax = 1;


    public float ammoSpawnIntervalMin = 0f;

    public float ammoSpawnIntervalMax = 0f;

    [Space(10)] [Header("Ammo Trail Details")]
    public bool isAmmoTrail = false;

    public float ammoTrialTime = 3f;
    public Material ammoTrialMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0) {
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        }

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax),
            ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax),
            ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin,
            nameof(ammoSpawnAmountMax),
            ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin,
            nameof(ammoSpawnIntervalMax),
            ammoSpawnIntervalMax, true);
        if (isAmmoTrail) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrialTime), ammoTrialTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrialMaterial), ammoTrialMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }
#endif

    #endregion
}
