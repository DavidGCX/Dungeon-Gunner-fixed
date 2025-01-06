using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour {
    private bool isLit = false;
    private Door door;

    private void Awake() {
        door = GetComponentInParent<Door>();
    }

    public void FadeInDoor(Door door) {
        Material material = new Material(GameResources.Instance.variableLitShader);
        DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();
        if (!doorLightingControl.isLit) {
            SpriteRenderer[] spriteRenderers = GetComponentsInParent<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers) {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
                doorLightingControl.isLit = true;
            }
        }
    }

    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material) {
        spriteRenderer.material = material;
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime) {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        FadeInDoor(door);
        if (door.isLocked && other.CompareTag(Settings.playerTag)) {
            GameManager.Instance.messageStack.AddMessage("The door is locked", MessageType.Warning);
        }
    }
}
