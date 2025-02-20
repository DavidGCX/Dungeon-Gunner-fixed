using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour {
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        if (audioSource.clip) {
            audioSource.Play();
        }
    }

    private void OnDisable() {
        audioSource.Stop();
    }

    public void SetSound(SoundEffectSO soundEffect) {
        audioSource.pitch = UnityEngine.Random.Range(soundEffect.soundEffectPitchRandomVariationMin,
            soundEffect.soundEffectPitchRandomVariationMax);
        audioSource.volume = soundEffect.soundEffectVolume;
        audioSource.clip = soundEffect.soundEffectClip;
    }
}
