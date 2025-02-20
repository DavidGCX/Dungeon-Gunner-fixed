using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject {
    [Header("Sound Effect Details")] public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(0.1f, 1.5f)] public float soundEffectPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)] public float soundEffectPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)] public float soundEffectVolume = 1f;

    #region Validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin),
            soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax),
            soundEffectPitchRandomVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }

    #endregion
}
