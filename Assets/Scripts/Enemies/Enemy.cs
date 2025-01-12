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
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour {
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;

    private void Awake() {
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
