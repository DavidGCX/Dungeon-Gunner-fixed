using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager> {
    // Start is called before the first frame update
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    public GameState gameState;

    [Header("Testing only need to integrate to Game UI later")]
    public int GameSeed = 12345678;

    public bool RandomGeneratingNodeGraph = false;

    private void Start() {
        gameState = GameState.gameStarted;
    }

    private void Update() {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameState.gameStarted;
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            gameState = GameState.gameStartedWithSeed;
        }
    }


    private void HandleGameState() {
        switch (gameState) {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
            case GameState.gameStartedWithSeed:
                UnityEngine.Random.InitState(GameManager.Instance.GameSeed);
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex) {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(
            dungeonLevelList[dungeonLevelListIndex]);
        if (!dungeonBuiltSuccessfully) {
            Debug.LogError("Dungeon not built successfully");
        }
    }
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this,
            nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}