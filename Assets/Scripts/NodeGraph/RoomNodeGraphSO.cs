using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private void LoadRoomNodeDictionary() {
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

    public void GenerateEntrance() {
        RoomNodeSO entranceRoomNode =
            ScriptableObject.CreateInstance<RoomNodeSO>();
        entranceRoomNode.Initialize(
            new Rect(new Vector2(0, 0), new Vector2(160, 75)),
            this, roomNodeTypeList.GetRoomNodeTypeFromName("Entrance"));
    }
    public void GenerateDungeonGraphTest() {
        Queue<RoomNodeSO> roomNodeQueue = new Queue<RoomNodeSO>();
        roomNodeQueue.Enqueue(GetRoomNode(roomNodeTypeList.GetRoomNodeTypeFromName("Entrance")));
        
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
