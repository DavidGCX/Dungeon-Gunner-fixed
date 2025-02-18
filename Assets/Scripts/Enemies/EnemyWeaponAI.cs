using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour {
    [Tooltip("For bullet non passable layers")]
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Transform weaponShootPosition;
    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void Start() {
        enemyDetails = enemy.enemyDetails;
        firingIntervalTimer = GetWeaponFiringInterval();
        firingDurationTimer = GetWeaponFiringDuration();
    }

    private void Update() {
        firingIntervalTimer -= Time.deltaTime;
        if (firingIntervalTimer < 0f) {
            firingDurationTimer -= Time.deltaTime;
            if (firingDurationTimer > 0f) {
                firingDurationTimer -= Time.deltaTime;
                Fire();
            } else {
                firingDurationTimer = GetWeaponFiringDuration();
                firingIntervalTimer = GetWeaponFiringInterval();
            }
        }
    }

    private void Fire() {
        Vector3 playerDirection = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        Vector3 weaponDirection = GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position;
        if (weaponDirection.magnitude < Settings.useAimAngleDistance) {
            weaponDirection = playerDirection;
        }
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        AimDirection aimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        enemy.aimWeaponEvent.CallAimWeaponEvent(aimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        if (enemyDetails.enemyWeapon is not null) {
            float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            if (playerDirection.magnitude <= enemyAmmoRange) {
                if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;
                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, aimDirection, enemyAngleDegrees,
                    weaponAngleDegrees, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange) {
        RaycastHit2D hit = Physics2D.Raycast(weaponShootPosition.position, weaponDirection, enemyAmmoRange, layerMask);
        return hit && hit.transform.CompareTag(Settings.playerTag);
    }


    private float GetWeaponFiringDuration() {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    private float GetWeaponFiringInterval() {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }


    #region validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }

    #endregion
}
