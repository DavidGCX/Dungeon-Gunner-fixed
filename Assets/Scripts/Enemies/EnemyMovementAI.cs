using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour {
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;

    private void Awake() {
        enemy = GetComponent<Enemy>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start() {
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update() {
        MoveEnemy();
    }

    private void MoveEnemy() {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.player.GetPlayerPosition()) <
            enemy.enemyDetails.chaseDistance) {
            chasePlayer = true;
        }

        if (!chasePlayer) return;
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance
                .GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath)) {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
            CreatePath();
            if (movementSteps != null) {
                if (moveEnemyRoutine != null) {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps) {
        while (movementSteps.Count > 0) {
            Vector3 nextPosition = movementSteps.Pop();
            while (Vector3.Distance(transform.position, nextPosition) > 0.2f) {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position,
                    moveSpeed, (nextPosition - transform.position).normalized);
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent(); // enemy has reached the player
    }

    private void CreatePath() {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Grid grid = currentRoom.instantiatedRoom.grid;
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);
        if (movementDetails != null) {
            movementSteps.Pop(); // Remove the first step as it is the current position of the enemy
        } else {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom) {
        // Dealing with the half collision tile
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);
        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds
                .x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);
        int obstaclePenalty = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x,
            adjustedPlayerCellPosition.y];
        if (obstaclePenalty != 0) {
            return playerCellPosition;
        } else {
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (i == 0 && j == 0) continue;
                    Vector2Int newCellPosition = new Vector2Int(adjustedPlayerCellPosition.x + i,
                        adjustedPlayerCellPosition.y + j);
                    // Handle Out of bounds case
                    if (newCellPosition.x < 0 || newCellPosition.x >= currentRoom.templateWidth ||
                        newCellPosition.y < 0 || newCellPosition.y >= currentRoom.templateHeight) {
                        continue;
                    }

                    if (currentRoom.instantiatedRoom.aStarMovementPenalty[newCellPosition.x, newCellPosition.y] != 0) {
                        return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
                    }
                }
            }
        }

        return playerCellPosition;
    }

    #region validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

    #endregion
}
