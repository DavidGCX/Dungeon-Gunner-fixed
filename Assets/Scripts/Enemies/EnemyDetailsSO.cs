using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject {
    [Space(10)] [Header("Base Enemy Details")]
    public string enemyName;

    public GameObject enemyPrefab;

    public float chaseDistance = 50f;

    [Space(10)] [Header("Enemy Material")] public Material enemyStandardMaterial;

    [Space(10)] [Header("Enemy Materialize Settings")]
    public float enemyMaterializeTime = 0.5f;
    public Shader enemyMaterializeShader;
    [FormerlySerializedAs("enemyMaterilizeColor")] [ColorUsage(true, true)]
    public Color enemyMaterializeColor;

    [Space(10)] [Header("Enemy Weapon Settings")]
    public WeaponDetailsSO enemyWeapon;

    public float firingIntervalMin = 0.1f;
    public float firingIntervalMax = 1f;
    public float firingDurationMin = 1f;
    public float firingDurationMax = 2f;

    public bool firingLineOfSightRequired;

    [Space(10)] [Header("Enemy Health Settings")]
    public EnemyHealthDetails[] EnemyHealthDetailsArray;

    public bool isImmuneAfterHit = false;

    public float hitImmunityTime = 0.5f;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof
            (firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof
            (firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(EnemyHealthDetailsArray), EnemyHealthDetailsArray);
        if (isImmuneAfterHit) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif

    #endregion
}
