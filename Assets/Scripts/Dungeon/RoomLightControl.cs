using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightControl : MonoBehaviour {
    private InstantiatedRoom instantiatedRoom;

    private void Awake() {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj) {
        if (obj.room == instantiatedRoom.room && !instantiatedRoom.room.isLit) {
            FadeInRoomLight();
            FadeInDoors();
            instantiatedRoom.room.isLit = true;
        }
    }

    private void FadeInDoors() {
        Door[] doors = GetComponentsInChildren<Door>();

        foreach (Door door in doors) {
            door.GetComponentInChildren<DoorLightingControl>().FadeInDoor(door);
        }
    }

    private void FadeInRoomLight() {
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom room) {
        Material material = new Material(GameResources.Instance.variableLitShader);
        room.groudTilemap.GetComponent<TilemapRenderer>().material = material;
        room.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        room.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        room.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        room.minimapTilemap.GetComponent<TilemapRenderer>().material = material;
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime) {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        room.groudTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        room.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        room.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        room.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        room.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }
}
