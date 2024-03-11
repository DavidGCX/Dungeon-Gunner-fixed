using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeList", menuName = "NodeGraph/RoomNodeTypeList")]
public class RoomNodeTypeListSO : ScriptableObject {
    [Space(10)]
    [Header("List of Room Node Types")]
    [Tooltip(
        "This list should be populated with all the node type for the game, it is used instead of enum")]
    public List<RoomNodeTypeSO> list = new();

    public RoomNodeTypeSO GetRoomNodeTypeFromName(string name) {
        foreach (var roomNodeType in list) {
            if (roomNodeType.roomNodeTypeName == name) {
                return roomNodeType;
            }
        }

        return null;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this, "list", list);
    }

#endif

    #endregion"
}