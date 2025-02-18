using System;
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHearts = new List<GameObject>();


    private void OnEnable() {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthUI_OnHealthChanged;
    }

    private void OnDisable() {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthUI_OnHealthChanged;
    }

    private void HealthUI_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2) {
        UpdateHealthUI(arg2.healthPercent);
    }

    private void UpdateHealthUI(float arg2HealthPercent) {
        // TODO: should be refactor to set active not create every time
        foreach (GameObject healthHeart in healthHearts) {
            Destroy(healthHeart);
        }
        healthHearts.Clear();

        int healthHeartCount = Mathf.CeilToInt(arg2HealthPercent * 100f / 20f);
        for (int i = 0; i < healthHeartCount; i++) {
            GameObject healthHeart = Instantiate(GameResources.Instance.healthIconPrefab, this.transform);
            // TODO: Need to Refactor this using the UI Layout Group
            healthHeart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);
            healthHearts.Add(healthHeart);
        }

    }
}
