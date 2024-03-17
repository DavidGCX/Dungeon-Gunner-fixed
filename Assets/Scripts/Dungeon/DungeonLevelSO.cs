using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DungeonLevel_",
    menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject {
    public string levelName;

    public List<RoomTemplateSO> roomTemplateList;

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName),
            levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this,
                nameof(roomTemplateList), roomTemplateList)) {
            return;
        }

        if (HelperUtilities.ValidateCheckEnumerableValues(this,
                nameof(roomNodeGraphList), roomNodeGraphList)) {
            return;
        }

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList) {
            if (roomTemplateSO == null) {
                Debug.LogError("RoomTemplateSO is null in " + name);
                return;
            }

            if (roomTemplateSO.roomNodeType.isCorridorEW) {
                isEWCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isCorridorNS) {
                isNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance) {
                isEntrance = true;
            }
        }

        if (!isEWCorridor) {
            Debug.LogError("No EW corridor found in " + name);
        }

        if (!isNSCorridor) {
            Debug.LogError("No NS corridor found in " + name);
        }

        if (!isEntrance) {
            Debug.LogError("No entrance found in " + name);
        }


        foreach (RoomNodeGraphSO roomNodeGraphSO in roomNodeGraphList) {
            if (roomNodeGraphSO == null) {
                Debug.LogError("RoomNodeGraphSO is null in " + name);
                return;
            }

            foreach (RoomNodeSO roomNodeSO in roomNodeGraphSO.roomNodeList) {
                if (roomNodeSO == null) {
                    Debug.LogError("RoomNodeSO is null in " + name);
                    return;
                }

                if (roomNodeSO.roomNodeType.isCorridorEW ||
                    roomNodeSO.roomNodeType.isCorridorNS
                    || roomNodeSO.roomNodeType.isEntrance || roomNodeSO
                        .roomNodeType.isNone ||
                    roomNodeSO.roomNodeType.isCorridor) {
                    // since we already check for the template above
                    continue;
                }

                bool foundTemplateForRoomNode = false;
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList) {
                    if (roomTemplateSO.roomNodeType ==
                        roomNodeSO.roomNodeType) {
                        foundTemplateForRoomNode = true;
                        break;
                    }
                }

                if (!foundTemplateForRoomNode) {
                    Debug.LogError("No template found for " +
                                   roomNodeSO.roomNodeType.name + " in " +
                                   name);
                }
            }
        }
    }
#endif

    #endregion
}
