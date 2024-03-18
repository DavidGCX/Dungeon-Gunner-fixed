using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour {
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groudTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;


    private BoxCollider2D boxCollider2D;

    private void Awake() {
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
    }

    public void Initialize(GameObject roomGameObject) {
        PopulateTilemapMemberVariables(roomGameObject);
        BlockOffUnusedDoorWays();
        DisableCollisionTilemapRenderer();
    }

    private void BlockOffUnusedDoorWays() {
        foreach (var doorway in room.doorwayList) {
            if (doorway.isConnected) {
                continue;
            }

            if (collisionTilemap) {
                BlockADoorWayOnTilemaplayer(collisionTilemap, doorway);
            }

            if (groudTilemap) {
                BlockADoorWayOnTilemaplayer(groudTilemap, doorway);
            }

            if (minimapTilemap) {
                BlockADoorWayOnTilemaplayer(minimapTilemap, doorway);
            }

            if (frontTilemap) {
                BlockADoorWayOnTilemaplayer(frontTilemap, doorway);
            }

            if (decoration1Tilemap) {
                BlockADoorWayOnTilemaplayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap) {
                BlockADoorWayOnTilemaplayer(decoration2Tilemap, doorway);
            }
        }
    }

    private void BlockADoorWayOnTilemaplayer(Tilemap tilemap, Doorway doorway) {
        switch (doorway.orientation) {
            case Orientation.north:
            case Orientation.south:
                BlockDoorWayHorizontally(tilemap, doorway);
                break;
            case Orientation.west:
            case Orientation.east:
                BlockDoorWayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
        }
    }

    private void BlockDoorWayVertically(Tilemap tilemap, Doorway doorway) {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new
                    Vector3Int(startPosition.x + xPos,
                        startPosition.y - yPos, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - 1 - yPos, 0), tilemap.GetTile(
                    new Vector3Int(startPosition.x + xPos,
                        startPosition.y - yPos, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(
                    startPosition.x + xPos,
                    startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    private void BlockDoorWayHorizontally(Tilemap tilemap, Doorway doorway) {
        Vector2Int startCopyPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new
                    Vector3Int(startCopyPosition.x + xPos,
                        startCopyPosition.y - yPos, 0));
                tilemap.SetTile(new Vector3Int(startCopyPosition.x + 1 + xPos,
                    startCopyPosition.y - yPos, 0), tilemap.GetTile(
                    new Vector3Int(startCopyPosition.x + xPos,
                        startCopyPosition.y - yPos, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(
                    startCopyPosition.x + 1 + xPos,
                    startCopyPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    private void DisableCollisionTilemapRenderer() {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>()
                .enabled =
            false;
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameObject) {
        grid = roomGameObject.GetComponentInChildren<Grid>();
        Tilemap[] tilemaps =
            roomGameObject.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps) {
            if (tilemap.gameObject.CompareTag("groundTilemap")) {
                groudTilemap = tilemap;
            } else if
                (tilemap.gameObject.CompareTag("decoration1Tilemap")) {
                decoration1Tilemap = tilemap;
            } else if
                (tilemap.gameObject.CompareTag("decoration2Tilemap")) {
                decoration2Tilemap = tilemap;
            } else if (tilemap.gameObject.CompareTag("frontTilemap")) {
                frontTilemap = tilemap;
            } else if (tilemap.gameObject.CompareTag("collisionTilemap")) {
                collisionTilemap = tilemap;
            } else if (tilemap.gameObject.CompareTag("minimapTilemap")) {
                minimapTilemap = tilemap;
            }
        }
    }
}
