using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRoom : MonoBehaviour
{
    [Header("Minimap Camera")]
    [SerializeField] private Camera minimapCamera;

    private void Start() {
        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms() {
        foreach (var keyPair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary) {
            Room room = keyPair.Value;
            HelperUtilities.CameraWorldPositionBounds(minimapCamera, out Vector2 lowerounds, out Vector2 upperBounds);

            if (room.lowerBounds.x <= upperBounds.x && room.upperBounds.x >= lowerounds.x && room.lowerBounds.y <= upperBounds.y && room.upperBounds.y >= lowerounds.y) {
                room.instantiatedRoom.gameObject.SetActive(true);
            } else {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapCamera), minimapCamera);
    }

    #endregion
}
