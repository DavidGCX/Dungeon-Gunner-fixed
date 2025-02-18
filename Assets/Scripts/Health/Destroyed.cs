using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    private DestroyedEvent destroyedEvent;

    private void Awake() {
        destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable() {
        destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroyed;
    }

    private void OnDisable() {
        destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs args) {
        Debug.Log("Destroyed: " + this.gameObject.name);
        if (args.isPlayerDied) {
            gameObject.SetActive(false);
        } else {
            Destroy(gameObject);
        }
    }
}
