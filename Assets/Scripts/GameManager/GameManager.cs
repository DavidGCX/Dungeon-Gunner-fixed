using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameState gameState;
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
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj) {
        SetCurrentRoom(obj.room);
    }

    private void Start() {
        gameState = GameState.gameStarted;
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
