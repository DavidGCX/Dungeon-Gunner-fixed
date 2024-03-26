using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomNode", menuName = "NodeGraph/RoomNode")]
public class RoomNodeSO : ScriptableObject {
    [HideInInspector] public string id;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    [HideInInspector] public List<string> parentNodes = new();
    [HideInInspector] public List<string> childNodes = new();

    public void Initialize(RoomNodeGraphSO nodeGraph,
        RoomNodeTypeSO roomNodeType) {
        this.roomNodeType = roomNodeType;
        this.roomNodeGraph = nodeGraph;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode_" + roomNodeType.roomNodeTypeName;
        this.rect = new Rect(new Vector2(0, 0), new Vector2(160, 75));
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void ConnectChildNode(RoomNodeSO childNode) {
        // Not safe, for random generation only
        if (childNode == null) {
            return;
        }

        childNode.parentNodes.Add(id);
        childNodes.Add(childNode.id);
    }

    public bool IsChildRoomValid(RoomNodeTypeSO roomNodeTypeTarget) {
        bool isConnectedBossNodeAlready = false;
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList) {
            if (roomNode.roomNodeType.isBossRoom &&
                roomNode.parentNodes.Count > 0) {
                isConnectedBossNodeAlready = true;
            }
        }

        if (roomNodeTypeTarget
                .isBossRoom &&
            isConnectedBossNodeAlready) {
            return false;
        }

        if (roomNodeTypeTarget.isNone) {
            return false;
        }

        if (roomNodeTypeTarget
            .isEntrance) {
            return false;
        }


        if (roomNodeTypeTarget
                .isCorridor ==
            roomNodeType.isCorridor) {
            return false;
        }


        if (roomNodeTypeTarget
                .isCorridor &&
            childNodes.Count >= Settings.MaxChildCorridors) {
            return false;
        }

        if (childNodes.Count > 0 && !roomNodeTypeTarget.isCorridor) {
            return false;
        }

        return true;
    }

    #region Editor Code

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Draw(GUIStyle nodeStyle) {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        if (parentNodes.Count > 0 || roomNodeType.isEntrance) {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName,
                new GUIStyle() {
                    fontSize = 14, fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState() {
                        textColor = Color.white
                    }
                });
        } else {
            int selectedIndex = roomNodeTypeList.list.IndexOf(roomNodeType);
            int selection =
                EditorGUILayout.Popup("", selectedIndex,
                    GetRoomNodeTypeToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];

            List<RoomNodeSO> nodesToRemoveConnection = new List<RoomNodeSO>();
            foreach (var childNodeID in childNodes) {
                if (!IsChildRoomValid(childNodeID)) {
                    RoomNodeSO childNode =
                        roomNodeGraph.GetRoomNodeFromID(childNodeID);
                    if (childNode) {
                        nodesToRemoveConnection.Add(childNode);
                    }
                }
            }

            foreach (var node in nodesToRemoveConnection) {
                node.RemoveSelfFromParentRoomNode(id);
            }
        }


        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(this);
        }

        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypeToDisplay() {
        string[] roomNodeTypeNames = new string[roomNodeTypeList.list.Count];
        for (int i = 0; i < roomNodeTypeList.list.Count; i++) {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor) {
                roomNodeTypeNames[i] =
                    roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomNodeTypeNames;
    }

    public void ProcessEvents(Event currentEvent) {
        switch (currentEvent.type) {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent) {
        if (currentEvent.button == 0) {
            ProcessMouseLeftClickDragEvent(currentEvent);
        }
    }

    private void ProcessMouseLeftClickDragEvent(Event currentEvent) {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    private void ProcessMouseUpEvent(Event currentEvent) {
        if (currentEvent.button == 0) {
            ProcessMouseLeftClickUpEvent(currentEvent);
        }
    }

    private void ProcessMouseLeftClickUpEvent(Event currentEvent) {
        isLeftClickDragging = false;
    }

    private void ProcessMouseDownEvent(Event currentEvent) {
        if (currentEvent.button == 0) {
            ProcessLeftClickDownEvent(currentEvent);
        } else if (currentEvent.button == 1) {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent) {
        roomNodeGraph.SetNodeToDrawLineFrom(this, currentEvent.mousePosition);
    }

    private void ProcessLeftClickDownEvent(Event currentEvent) {
        Selection.activeObject = this;
        isSelected = !isSelected;
    }

    public void DragNode(Vector2 delta) {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public void Initialize(Rect rect, RoomNodeGraphSO nodeGraph,
        RoomNodeTypeSO roomNodeType) {
        this.roomNodeType = roomNodeType;
        this.roomNodeGraph = nodeGraph;
        this.id = Guid.NewGuid().ToString();
        this.rect = rect;
        this.name = "RoomNode";
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool AddChildNodeIDToRoomNode(string childNodeID) {
        if (childNodes.Contains(childNodeID)) {
            return false;
        }

        if (IsChildRoomValid(childNodeID)) {
            childNodes.Add(childNodeID);
            return true;
        }

        return false;
    }


    public bool IsChildRoomValid(string childNodeID) {
        if (roomNodeGraph.GetRoomNodeFromID(childNodeID) == null) {
            return false;
        }

        bool isConnectedBossNodeAlready = false;
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList) {
            if (roomNode.roomNodeType.isBossRoom &&
                roomNode.parentNodes.Count > 0) {
                isConnectedBossNodeAlready = true;
            }

            ;
        }

        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).roomNodeType
                .isBossRoom &&
            isConnectedBossNodeAlready) {
            return false;
        }

        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).roomNodeType.isNone) {
            return false;
        }

        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).roomNodeType
            .isEntrance) {
            return false;
        }

        if (childNodes.Contains(childNodeID)) {
            return false;
        }

        if (id == childNodeID) {
            return false;
        }

        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).parentNodes.Count >
            0) {
            return false;
        }

        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).roomNodeType
                .isCorridor ==
            roomNodeType.isCorridor) {
            return false;
        }


        if (roomNodeGraph.GetRoomNodeFromID(childNodeID).roomNodeType
                .isCorridor &&
            childNodes.Count >= Settings.MaxChildCorridors) {
            return false;
        }

        if (childNodes.Count > 0 && !roomNodeGraph
                .GetRoomNodeFromID(childNodeID)
                .roomNodeType.isCorridor) {
            return false;
        }

        return true;
    }

    public bool AddParentNodeIDToRoomNode(string parentNodeID) {
        if (parentNodes.Contains(parentNodeID)) {
            return false;
        }

        parentNodes.Add(parentNodeID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childNodeID) {
        if (childNodes.Contains(childNodeID)) {
            childNodes.Remove(childNodeID);
            return true;
        }

        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentNodeID) {
        if (parentNodes.Contains(parentNodeID)) {
            parentNodes.Remove(parentNodeID);
            return true;
        }

        return false;
    }

    public bool RemoveSelfFromParentRoomNode(string parentNodeID) {
        RoomNodeSO parentNode = roomNodeGraph.GetRoomNodeFromID(parentNodeID);
        if (parentNode != null && parentNode.childNodes.Contains(id)) {
            parentNode.RemoveChildRoomNodeIDFromRoomNode(id);
            RemoveParentRoomNodeIDFromRoomNode(parentNodeID);
            return true;
        }

        return false;
    }

    public void ClearConnectionBetweenSelectedNode() {
        List<RoomNodeSO> nodes = new List<RoomNodeSO>();
        foreach (string childNodeID in childNodes) {
            RoomNodeSO childNode = roomNodeGraph.GetRoomNodeFromID(childNodeID);
            if (childNode != null && childNode.parentNodes.Contains(id) &&
                childNode.isSelected) {
                nodes.Add(childNode);
            }
        }

        foreach (var child in nodes) {
            child.RemoveSelfFromParentRoomNode(id);
        }
    }

    public void ClearAllRelatedConnection() {
        List<RoomNodeSO> childNodesToRemoveConnection = new List<RoomNodeSO>();
        foreach (string childNodeID in childNodes) {
            RoomNodeSO childNode = roomNodeGraph.GetRoomNodeFromID(childNodeID);
            if (childNode != null && childNode.parentNodes.Contains(id)) {
                childNodesToRemoveConnection.Add(childNode);
            }
        }

        foreach (var childNode in childNodesToRemoveConnection) {
            RemoveSelfFromParentRoomNode(id);
        }

        List<RoomNodeSO> parentNodesToRemoveConnection = new List<RoomNodeSO>();
        foreach (string parentNodeID in parentNodes) {
            RoomNodeSO parentNode =
                roomNodeGraph.GetRoomNodeFromID(parentNodeID);
            if (parentNode != null && parentNode.childNodes.Contains(id)) {
                parentNodesToRemoveConnection.Add(parentNode);
            }
        }

        foreach (var parentNode in parentNodesToRemoveConnection) {
            RemoveSelfFromParentRoomNode(parentNode.id);
        }
    }
#endif

    #endregion
}