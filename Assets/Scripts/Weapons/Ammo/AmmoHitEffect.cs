using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour {
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake() {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO weaponShootEffect) {
        SetShootEffectColorGradient(weaponShootEffect.colorGradient);
        SetShootEffectParticleStartingValues(weaponShootEffect.duration, weaponShootEffect.startParticleSize,
            weaponShootEffect.startParticleSpeed, weaponShootEffect.startLifetime, weaponShootEffect.effectGravity,
            weaponShootEffect.maxParticleNumber);
        SetShootEffectParticleEmission(weaponShootEffect.emissionRate, weaponShootEffect.burstParticleNumber);
        SetShootEffectParticleSprite(weaponShootEffect.sprite);
        SetShootEffectVelocityOverLifetime(weaponShootEffect.velocityOverLifetimeMin,
            weaponShootEffect.velocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient colorGradient) {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetime.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticleNumber) {
        ParticleSystem.MainModule main = ammoHitEffectParticleSystem.main;
        main.duration = duration;
        main.startSize = startParticleSize;
        main.startSpeed = startParticleSpeed;
        main.startLifetime = startLifetime;
        main.maxParticles = maxParticleNumber;
        main.gravityModifier = effectGravity;
    }

    private void SetShootEffectParticleEmission(int emissionRate, int burstParticleNumber) {
        ParticleSystem.EmissionModule emission = ammoHitEffectParticleSystem.emission;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emission.SetBurst(0, burst);
        emission.rateOverTime = emissionRate;
    }

    private void SetShootEffectParticleSprite(Sprite sprite) {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimation =
            ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimation.SetSprite(0, sprite);
    }

    private void SetShootEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax) {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime =
            ammoHitEffectParticleSystem.velocityOverLifetime;
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifetimeMax.x;
        velocityOverLifetime.x = minMaxCurveX;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifetimeMax.y;
        velocityOverLifetime.y = minMaxCurveY;
    }
}
