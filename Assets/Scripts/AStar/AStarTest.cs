using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour {
    private InstantiatedRoom instantiatedRoom;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase finishPathTile;
    private Vector3Int novalue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> pathStack;

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start() {
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTileArray[0];
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj) {
        pathStack = null;
        instantiatedRoom = obj.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = novalue;
        endGridPosition = novalue;
        SetUpPathTileMap();
    }

    private void SetUpPathTileMap() {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");
        if (tilemapCloneTransform == null) {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
            pathTilemap.name = "Tilemap4_Front(Clone)";
        } else {
            pathTilemap = tilemapCloneTransform.GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    private void Update() {
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null ||
            pathTilemap == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            DisplayPath();
        }
    }

    private void SetEndPosition() {
        if (endGridPosition == novalue) {
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());
            if (!IsPositionWithinBounds(endGridPosition)) {
                endGridPosition = novalue;
                return;
            }

            pathTilemap.SetTile(endGridPosition, finishPathTile);
        } else {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = novalue;
        }
    }

    private void SetStartPosition() {
        if (startGridPosition == novalue) {
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());
            if (!IsPositionWithinBounds(startGridPosition)) {
                startGridPosition = novalue;
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        } else {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = novalue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int vector3Int) {
        return vector3Int.x >= instantiatedRoom.room.templateLowerBounds.x &&
               vector3Int.x <= instantiatedRoom.room.templateUpperBounds.x &&
               vector3Int.y >= instantiatedRoom.room.templateLowerBounds.y &&
               vector3Int.y <= instantiatedRoom.room.templateUpperBounds.y;
    }

    private void DisplayPath() {
        if (startGridPosition == novalue || endGridPosition == novalue) {
            return;
        }

        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);
        if (pathStack == null) {
            return;
        }

        foreach (var pathTile in pathStack) {
            pathTilemap.SetTile(grid.WorldToCell(pathTile), startPathTile);
        }
    }

    private void ClearPath() {
        if (pathStack == null) {
            return;
        }

        foreach (var pathTile in pathStack) {
            pathTilemap.SetTile(grid.WorldToCell(pathTile), null);
        }

        pathStack = null;

        endGridPosition = novalue;
        startGridPosition = novalue;
    }
}
