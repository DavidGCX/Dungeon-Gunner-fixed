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
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour {
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;

    private void Awake() {
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel) {
        this.enemyDetails = enemyDetails;
        SetEnemyUpdateFrameNumber(enemySpawnNumber);
        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());
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
            enemyDetails.enemyMaterilizeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray,
            enemyDetails.enemyStandardMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool enable) {
        circleCollider.enabled = enable;
        polygonCollider.enabled = enable;
        enemyMovementAI.enabled = enable;

    }
}
