using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scroeText;

    private void Awake() {
        scroeText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs args) {
        scroeText.text = "SCORE: " + args.score.ToString("###,###0");
    }
}
