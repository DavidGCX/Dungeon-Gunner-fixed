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

    public void SetCurrentRoom(Room room) {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    public Player GetPlayer() {
        return player;
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex) {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(
            dungeonLevelList[dungeonLevelListIndex]);
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
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this,
            nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
