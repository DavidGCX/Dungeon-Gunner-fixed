using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager> {
    // Start is called before the first frame update
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    public GameState gameState;

    private void Start() {
        gameState = GameState.gameStarted;
    }

    private void Update() {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameState.gameStarted;
        }
    }


    private void HandleGameState() {
        switch (gameState) {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex) {
    }
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this,
            nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
