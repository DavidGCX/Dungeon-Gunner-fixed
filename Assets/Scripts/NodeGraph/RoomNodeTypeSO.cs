using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType", menuName = "NodeGraph/RoomNodeType")]
public class RoomNodeTypeSO : ScriptableObject {
    public string roomNodeTypeName;
    public bool displayInNodeGraphEditor = true;

    public bool isCorridor;

    // Only used for room node template.
    public bool isCorridorNS;

    // Only used for room node template.
    public bool isCorridorEW;
    public bool isEntrance;
    public bool isBossRoom;
    public bool isNone;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, "roomNodeTypeName",
            roomNodeTypeName);
    }
#endif

    #endregion
}