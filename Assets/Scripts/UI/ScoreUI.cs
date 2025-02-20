using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable() {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs args) {
        scoreText.text = "SCORE: " + args.score.ToString("###,###0") + "\nMULTIPLIER: x" +
                         GameManager.Instance.scoreMultiplier;
    }
}
