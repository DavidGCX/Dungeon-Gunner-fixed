using System;
using UnityEngine;

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
    [ColorUsage(true, true)]
    public Color enemyMaterilizeColor;
    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);

    }
#endif

    #endregion
}
