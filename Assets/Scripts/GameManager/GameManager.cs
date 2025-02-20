using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager> {
    // Start is called before the first frame update
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    public Player player;

    public bool ghostMode = false;

    [HideInInspector] public GameState gameState;

    public void SetGameState(GameState gameState) {
        previousGameState = this.gameState;
        this.gameState = gameState;
    }

    public GameState previousGameState;
    private long gameScore;
    public MessageStack messageStack;

    [Header("Testing only need to integrate to Game UI later")]
    public int GameSeed = 12345678;

    public bool RandomGeneratingNodeGraph = false;

    protected override void Awake() {
        base.Awake();
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        InstantiatePlayer();
    }

    private void InstantiatePlayer() {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);
        player = playerGameObject.GetComponent<Player>();
        player.Initialized(playerDetails);
    }

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScore;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScore;
    }

    private void StaticEventHandler_OnPointsScore(PointsScoredArgs args) {
        gameScore += args.points;
        StaticEventHandler.CallScoreChangedEvent(gameScore);
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj) {
        messageStack.AddMessage("Room Cleared", MessageType.Event);
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj) {
        SetCurrentRoom(obj.room);
    }

    private void Start() {
        gameState = GameState.gameStarted;
        previousGameState = GameState.gameStarted;
        gameScore = 0;
    }

    [ButtonInvoke(nameof(RestartGameDebug))]
    public bool RestartGame;

    private void RestartGameDebug() {
        gameState = GameState.gameStarted;
    }

    [ButtonInvoke(nameof(RestartGameWithSeedDebug))]
    public bool RestartGameWithSeed;

    private void RestartGameWithSeedDebug() {
        gameState = GameState.gameStartedWithSeed;
    }

    public void TriggerGhostMode(bool isGhostMode) {
        player.SetImmortal(isGhostMode);
        player.GetComponent<PlayerControl>().SetMoveSpeed(50f);
        ghostMode = isGhostMode;
        if (!currentRoom.instantiatedRoom.room.roomNodeType.isCorridor && !currentRoom.instantiatedRoom.room
                .roomNodeType.isEntrance && isGhostMode) {
            currentRoom.instantiatedRoom.UnlockDoors();
        }

    }

    private void Update() {
        HandleGameState();
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

    public void SetCurrentRoom(Room room) {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    public Player GetPlayer() {
        return player;
    }

    public Sprite GetPlayerMinimapIcon() {
        return playerDetails.playerMinimapIcon;
    }

    public void DisplayMessage(string message, MessageType messageType) {
        messageStack.AddMessage(message, messageType);
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex) {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(
            dungeonLevelList[dungeonLevelListIndex]);
        messageStack.AddMessage("Dungeon built successfully", MessageType.Event);
        if (!dungeonBuiltSuccessfully) {
            Debug.LogError("Dungeon not built successfully");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set player position to the center of the current room
        var position = player.gameObject.transform.position;
        position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0);
        position =
            HelperUtilities.GetSpawnPointNearestToPlayer(position);
        player.gameObject.transform.position = position;
    }

    public Room GetCurrentRoom() {
        return currentRoom;
    }

    public DungeonLevelSO GetCurrentDungeonLevel() {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this,
            nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
