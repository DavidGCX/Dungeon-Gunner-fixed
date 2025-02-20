using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Runtime.InteropServices;

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(DealContactDamage))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour {
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public FireWeapon fireWeapon;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyedEvent destroyedEvent;

    private void Awake() {
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        materializeEffect = GetComponent<MaterializeEffect>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable() {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable() {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs) {
        if (healthEventArgs.healthAmount <= 0) {
            destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
        }
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel) {
        this.enemyDetails = enemyDetails;
        SetEnemyUpdateFrameNumber(enemySpawnNumber);
        SetEnemyStartingWeapon();
        SetEnemyAnimationSpeed();
        SetEnemyStartingHealth(dungeonLevel);
        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel) {
        foreach (EnemyHealthDetails enemyHealthDetail in enemyDetails.EnemyHealthDetailsArray) {
            if (enemyHealthDetail.dungeonLevel == dungeonLevel) {
                health.SetStartingHealth(enemyHealthDetail.enemyHealthAmount);
                break;
            }
        }

        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    private void SetEnemyStartingWeapon() {
        if (enemyDetails.enemyWeapon is not null) {
            Weapon weapon = new Weapon() {
                weaponDetails = enemyDetails.enemyWeapon, weaponReloadTimer = 0f, weaponClipRemainingAmmo =
                    enemyDetails.enemyWeapon.weaponClipAmmoCapacity,
                weaponRemainingAmmo = enemyDetails.enemyWeapon
                    .weaponAmmoCapacity,
                isWeaponReloading = false, owner = this.gameObject
            };
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    private void SetEnemyUpdateFrameNumber(int enemySpawnNumber) {
        enemyMovementAI.updateFrameNumber = enemySpawnNumber;
    }

    private void SetEnemyAnimationSpeed() {
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy() {
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader,
            enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray,
            enemyDetails.enemyStandardMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool enable) {
        circleCollider.enabled = enable;
        polygonCollider.enabled = enable;
        enemyMovementAI.enabled = enable;
        fireWeapon.enabled = enable;
    }
}
