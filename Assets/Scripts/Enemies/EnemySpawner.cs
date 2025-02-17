using System;
using System.Collections;
using System.Resources;
using UnityEngine;
using Random = UnityEngine.Random;


[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehavior<EnemySpawner> {
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs) {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;
        currentRoom = roomChangedEventArgs.room;

        if (currentRoom.isClearedOfEnemies) {
            return;
        }

        if (currentRoom.roomNodeType.isCorridor || currentRoom.roomNodeType.isEntrance) {
            return;
        }

        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        roomEnemySpawnParameters =
            currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        if (enemiesToSpawn == 0) {
            currentRoom.isClearedOfEnemies = true;
            return;
        }

        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        currentRoom.instantiatedRoom.LockDoors();

        SpawnEnemies();
    }

    private void SpawnEnemies() {
        if (GameManager.Instance.gameState == GameState.playingLevel) {
            GameManager.Instance.SetGameState(GameState.engagingEnemies);
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine() {
        Grid grid = currentRoom.instantiatedRoom.grid;

        RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject =
            new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        if (currentRoom.spawnPositionArray.Length > 0) {
            for (int i = 0; i < enemiesToSpawn; i++) {
                if (currentEnemyCount >= enemyMaxConcurrentSpawnNumber) {
                    yield return new WaitUntil(() => currentEnemyCount < enemyMaxConcurrentSpawnNumber);
                }

                Vector3Int spawnPosition = (Vector3Int)currentRoom.spawnPositionArray[UnityEngine.Random.Range(0,
                    currentRoom
                        .spawnPositionArray.Length)];
                CreateEnemy(randomSpawnableObject.GetItem(), grid.CellToWorld(spawnPosition));
                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetail, Vector3 spawnPosition) {
        currentEnemyCount++;
        enemiesSpawnedSoFar++;
        DungeonLevelSO currentLevel = GameManager.Instance.GetCurrentDungeonLevel();
        GameObject enemy = Instantiate(enemyDetail.enemyPrefab, spawnPosition, Quaternion.identity, transform);
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetail, enemiesSpawnedSoFar, currentLevel);
    }

    private float GetEnemySpawnInterval() {
        return Random.Range(roomEnemySpawnParameters.minSpawnInterval,
            roomEnemySpawnParameters.maxSpawnInterval);
    }

    private int GetConcurrentEnemies() {
        return Random.Range(roomEnemySpawnParameters.minConcurrentEnemies,
            roomEnemySpawnParameters.maxConcurrentEnemies + 1);
    }
}
