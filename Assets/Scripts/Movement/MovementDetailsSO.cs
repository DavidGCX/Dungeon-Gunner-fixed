using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MovementDetails", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject {
    [Space(10)] [Header("Movement Details")]
    public float minMoveSpeed = 8f;

    public float maxMoveSpeed = 8f;

    public float rollSpeed;
    public float rollDistance;
    public float rollCooldownTime;

    public float GetMoveSpeed() {
        const double tolerance = 0.0001f;
        return Math.Abs(minMoveSpeed - maxMoveSpeed) < tolerance
            ? minMoveSpeed
            : Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed),
            maxMoveSpeed, false);
        if (rollDistance != 0f || rollSpeed != 0f || rollCooldownTime != 0f) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif

    #endregion
}
