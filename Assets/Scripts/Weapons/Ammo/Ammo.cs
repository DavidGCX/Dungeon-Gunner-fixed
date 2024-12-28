using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable {
    [SerializeField] private TrailRenderer trailRenderer;
    private float ammoRange = 0f;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (ammoChargeTimer > 0) {
            ammoChargeTimer -= Time.deltaTime;
        } else if (!isAmmoMaterialSet) {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        Vector3 distanceVector = fireDirectionVector * (ammoSpeed * Time.deltaTime);
        transform.position += distanceVector;
        ammoRange -= distanceVector.magnitude;
        if (ammoRange <= 0) {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DisableAmmo();
    }

    private void DisableAmmo() {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material ammoDetailsAmmoMaterial) {
        spriteRenderer.material = ammoDetailsAmmoMaterial;
    }

    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false) {
        #region ammo

        this.ammoDetails = ammoDetails;
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
        spriteRenderer.sprite = ammoDetails.ammoSprite;
        this.ammoSpeed = ammoSpeed;

        if (ammoDetails.ammoChargeTime > 0) {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        } else {
            ammoChargeTimer = 0;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        ammoRange = ammoDetails.ammoRange;
        this.ammoSpeed = ammoSpeed;
        this.overrideAmmoMovement = overrideAmmoMovement;

        gameObject.SetActive(true);

        #endregion

        #region ammo trail

        if (ammoDetails.isAmmoTrail) {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.enabled = true;
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrialMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrialTime;
        } else {
            trailRenderer.emitting = false;
            trailRenderer.enabled = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetailsSo, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector) {
        float randomSpread = UnityEngine.Random.Range(ammoDetailsSo.ammoSpreadMin, ammoDetailsSo.ammoSpreadMax);
        // get 1 or -1 to determine the spread direction
        int spreadToggle = Random.Range(0, 2) * 2 - 1;
        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance) {
            fireDirectionAngle = aimAngle;
        } else {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += randomSpread * spreadToggle;

        transform.eulerAngles = new Vector3(0, 0, fireDirectionAngle);

        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    #region validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }

    #endregion
}
