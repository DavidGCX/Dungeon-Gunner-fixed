using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "RoomNodeGraph",
    menuName = "NodeGraph/RoomNodeGraph")]
public class RoomNodeGraphSO : ScriptableObject {
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();

    // Will use GUID to identify the nodes
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary =
        new Dictionary<string, RoomNodeSO>();

    public void Awake() {
        if (roomNodeList == null) {
            roomNodeList = new List<RoomNodeSO>();
        }

        if (roomNodeDictionary == null) {
            roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
        }

        roomNodeTypeList =
            GameResources.Instance.roomNodeTypeList;

        LoadRoomNodeDictionary();
    }

    public void LoadRoomNodeDictionary() {
        if (roomNodeDictionary == null) {
            roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
        }

        roomNodeDictionary.Clear();
        foreach (var roomNode in roomNodeList) {
            roomNodeDictionary.Add(roomNode.id, roomNode);
        }
    }

    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType) {
        foreach (var roomNode in roomNodeList) {
            if (roomNode.roomNodeType == roomNodeType) {
                return roomNode;
            }
        }

        return null;
    }

    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentNode) {
        foreach (string childNodeID in parentNode.childNodes) {
            yield return GetRoomNodeFromID(childNodeID);
        }
    }

    public void GenerateEntrance(bool saveToAsset = false) {
        if (roomNodeList == null) {
            roomNodeList = new List<RoomNodeSO>();
        }

        RoomNodeSO entranceRoomNode =
            ScriptableObject.CreateInstance<RoomNodeSO>();
        entranceRoomNode.Initialize(this,
            roomNodeTypeList.GetRoomNodeTypeFromName("Entrance"));
        this.roomNodeList.Add(entranceRoomNode);
#if UNITY_EDITOR
        if (saveToAsset) {
            AssetDatabase.AddObjectToAsset(entranceRoomNode, this);
        }
#endif
    }

    public void
        GenerateDungeonGraphTest(DungeonLevelRestriction dungeonLevelRestriction,
            bool saveToAsset = false) {
        Queue<RoomNodeSO> roomNodeQueue = new Queue<RoomNodeSO>();
        List<RoomNodeSO> totalRoomNodeListFromRestriction =
            GetTotalRoomNodeListFromRestriction(dungeonLevelRestriction);
        roomNodeQueue.Enqueue(
            GetRoomNode(roomNodeTypeList.GetRoomNodeTypeFromName("Entrance")));

        while (roomNodeQueue.Count > 0 && totalRoomNodeListFromRestriction.Count > 0) {
            RoomNodeSO currentNode = roomNodeQueue.Dequeue();
            int possibleChild =
                UnityEngine.Random.Range(0, Settings.MaxChildCorridors + 1);
            if (roomNodeQueue.Count == 0 && totalRoomNodeListFromRestriction.Count > 0 &&
                possibleChild == 0) {
                possibleChild = 1;
            }

            for (int i = 0; i < possibleChild; i++) {
                if (totalRoomNodeListFromRestriction.Count == 0) {
                    break;
                }

                RoomNodeSO roomNode = totalRoomNodeListFromRestriction[0];
                totalRoomNodeListFromRestriction.RemoveAt(0);
                roomNodeQueue.Enqueue(roomNode);
                CreateCorridorConnection(currentNode, roomNode, saveToAsset);
                this.roomNodeList.Add(roomNode);
#if UNITY_EDITOR
                if (saveToAsset) {
                    AssetDatabase.AddObjectToAsset(roomNode, this);
                }
#endif
            }
        }
    }

    private void CreateCorridorConnection(RoomNodeSO currentNode, RoomNodeSO roomNode,
        bool saveToAsset = false) {
        RoomNodeSO corridorNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        corridorNode.Initialize(this,
            roomNodeTypeList.GetRoomNodeTypeFromName("Corridor"));
        this.roomNodeList.Add(corridorNode);
        currentNode.ConnectChildNode(corridorNode);
        corridorNode.ConnectChildNode(roomNode);
#if UNITY_EDITOR
        if (saveToAsset) {
            AssetDatabase.AddObjectToAsset(corridorNode, this);
        }
#endif
    }

    private List<RoomNodeSO> GetTotalRoomNodeListFromRestriction(
        DungeonLevelRestriction dungeonLevelRestriction) {
        List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
        roomNodeList.AddRange(GetRoomNodeListFromRestriction(
            dungeonLevelRestriction.minSmallRoomCount,
            dungeonLevelRestriction.maxSmallRoomCount,
            roomNodeTypeList.GetRoomNodeTypeFromName("Small Room")));
        roomNodeList.AddRange(GetRoomNodeListFromRestriction(
            dungeonLevelRestriction.minMediumRoomCount,
            dungeonLevelRestriction.maxMediumRoomCount,
            roomNodeTypeList.GetRoomNodeTypeFromName("Medium Room")));
        roomNodeList.AddRange(GetRoomNodeListFromRestriction(
            dungeonLevelRestriction.minLargeRoomCount,
            dungeonLevelRestriction.maxLargeRoomCount,
            roomNodeTypeList.GetRoomNodeTypeFromName("Large Room")));
        roomNodeList.AddRange(GetRoomNodeListFromRestriction(
            dungeonLevelRestriction.minChestRoomCount,
            dungeonLevelRestriction.maxChestRoomCount,
            roomNodeTypeList.GetRoomNodeTypeFromName("Chest Room")));
        ShuffleList(roomNodeList);
        roomNodeList.AddRange(GetRoomNodeListFromRestriction(
            dungeonLevelRestriction.minBossRoomCount,
            dungeonLevelRestriction.maxBossRoomCount,
            roomNodeTypeList.GetRoomNodeTypeFromName("Boss Room")));
        return roomNodeList;
    }

    private void ShuffleList(List<RoomNodeSO> roomNodeSos) {
        int n = roomNodeSos.Count;
        while (n > 1) {
            n--;
            var k = UnityEngine.Random.Range(0, roomNodeSos.Count);
            (roomNodeSos[k], roomNodeSos[n]) = (roomNodeSos[n], roomNodeSos[k]);
        }
    }

    private IEnumerable<RoomNodeSO> GetRoomNodeListFromRestriction(
        int minSmallRoomCount,
        int maxSmallRoomCount, RoomNodeTypeSO getRoomNodeTypeFromName) {
        for (int i = 0;
             i < UnityEngine.Random.Range(minSmallRoomCount, maxSmallRoomCount);
             i++) {
            RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
            roomNode.Initialize(this, getRoomNodeTypeFromName);
            yield return roomNode;
        }
    }

    public RoomNodeSO GetRoomNodeFromID(string id) {
        if (roomNodeDictionary.TryGetValue(id, out var roomNodeFromID)) {
            return roomNodeFromID;
        }

        return null;
    }

#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;

    [HideInInspector] public Vector2 linePosition;
    [HideInInspector] public Vector2 selectionBoxStartPosition;
    [HideInInspector] public Vector2 selectionBoxEndPosition;
    [HideInInspector] public bool isDraggingSelectionBox = false;

    public void OnValidate() {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawLineFrom(RoomNodeSO roomNode,
        Vector2 lineEndPosition) {
        roomNodeToDrawLineFrom = roomNode;
        this.linePosition = lineEndPosition;
    }

    public bool IsWithinSelectionBounds(RoomNodeSO roomNode) {
        if (!isDraggingSelectionBox) {
            return false;
        }

        return roomNode.rect.center.x > MathF.Min(selectionBoxStartPosition.x,
                   selectionBoxEndPosition.x) && roomNode.rect.center.x <
               MathF.Max(selectionBoxStartPosition.x,
                   selectionBoxEndPosition.x) &&
               roomNode.rect.center.y > MathF.Min(selectionBoxStartPosition.y,
                   selectionBoxEndPosition.y) && roomNode.rect.center.y <
               MathF.Max(selectionBoxStartPosition.y,
                   selectionBoxEndPosition.y);
    }
#endif
}