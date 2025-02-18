using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour {
    private int startingHealth;
    private int currentHealth;
    private bool isImmortal;
    private HealthEvent healthEvent;
    private bool immuneToDamage;

    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit;
    private float immuneTime = 1f;
    private SpriteRenderer spriteRenderer;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds spriteFlashWait = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;

    // TODO: Refactor player and enemy code to use the same parent and not have duplicate code here
    private Player player;
    private Enemy enemy;
    private void Awake() {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start() {
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        if (player) {
            isImmuneAfterHit = player.playerDetails.isImmuneAfterHit;
            immuneTime = player.playerDetails.hitImmunityDuration;
            spriteRenderer = player.spriteRenderer;

        } else if (enemy) {
            isImmuneAfterHit = enemy.enemyDetails.isImmuneAfterHit;
            immuneTime = enemy.enemyDetails.hitImmunityTime;
            spriteRenderer = enemy.spriteRendererArray[0];
        }
    }
    public void SetImmuneToDamage(bool immuneToDamage) {
        this.immuneToDamage = immuneToDamage;
    }

    public void TakeDamage(int damageAmount, Weapon damageSource) {
        if (immuneToDamage) {
            Debug.Log(this.gameObject.name + " is immune to damage");
        }

        if (isDamageable && !immuneToDamage) {
            currentHealth -= damageAmount;
            // Debug.Log(this.gameObject.name + " Health: " + currentHealth + "Damage From: " + damageSource.owner);
            CallHealthEvent(damageAmount);

            PostHitImmunity();
        }
    }
    public void TakeDamage(int damageAmount, GameObject damageSource) {
        if (immuneToDamage) {
            Debug.Log(this.gameObject.name + " is immune to damage");
        }

        if (isDamageable && !immuneToDamage) {
            currentHealth -= damageAmount;
            // Debug.Log(this.gameObject.name + " Health: " + currentHealth + "Damage From: " + damageSource.owner);
            CallHealthEvent(damageAmount);

            PostHitImmunity();
        }
    }

    private void PostHitImmunity() {
        if (!gameObject.activeSelf) return;

        if (isImmuneAfterHit) {
            if (immunityCoroutine != null) {
                StopCoroutine(immunityCoroutine);
            }
            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immuneTime, spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityRoutine(float duration, SpriteRenderer spriteRenderer) {
        int flashTimes = Mathf.RoundToInt(duration / spriteFlashInterval / 2f);
        immuneToDamage = true;
        for (int i = 0; i < flashTimes; i++) {
            spriteRenderer.color = Color.red;
            yield return spriteFlashWait;
            spriteRenderer.color = Color.white;
            yield return spriteFlashWait;
        }
        immuneToDamage = false;
        yield return null;
    }


    private void CallHealthEvent(int damageAmount) {
        healthEvent.CallHealthChangedEvent((float)currentHealth/ (float)startingHealth, currentHealth ,damageAmount);
    }

    public void SetStartingHealth(int startingHealth) {
        this.startingHealth = startingHealth;
        currentHealth = this.startingHealth;
    }

    public int GetStartingHealth() {
        return startingHealth;
    }

    public void SetImmortal(bool isImmortal) {
        this.isImmortal = isImmortal;
    }
}
