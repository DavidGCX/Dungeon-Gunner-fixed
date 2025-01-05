using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour {
    [SerializeField] private BoxCollider2D doorCollider;
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;
    private bool isLocked = false;

    private void Awake() {
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
        doorCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(Settings.playerTag) || other.CompareTag(Settings.playerWeapon)) {
            OpenDoor();
        }
        if(isLocked) {

        }
    }

    private void OnEnable() {
        //restore animator state
        animator.SetBool(Settings.open, isOpen);
    }

    public bool IsOpen() {
        return isOpen;
    }

    public void OpenDoor() {
        if (!isOpen) {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenSoundEffect);
        }
    }

    public void LockDoor() {
        isLocked = true;
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnLockDoor() {
        isLocked = false;
        doorCollider.enabled = false;
        doorTrigger.enabled = true;
        if (previouslyOpened) {
            isOpen = false;
            OpenDoor();
        }
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif

    #endregion
}
