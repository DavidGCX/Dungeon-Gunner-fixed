using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental;
using UnityEditor.Graphs;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

public class RoomNodeGraphEditor : EditorWindow
{
    private const float NodeWidth = 160f;
    private const float NodeHeight = 75f;
    private const int NodePadding = 27;
    private const int NodeBorder = 12;
    private const float connectingLineWidth = 3f;
    private const float connectingArrowWidth = 8f;
    private GUIStyle _roomNodeSyle;
    private GUIStyle selectedRoomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    // For drawing graph with grid
    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    private void OnEnable() {
        Selection.selectionChanged += InspectorSelectionChanged;
        _roomNodeSyle = new GUIStyle();
        _roomNodeSyle.normal.background =
            EditorGUIUtility.Load("node1") as Texture2D;
        _roomNodeSyle.border =
            new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);
        _roomNodeSyle.padding =
            new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
        _roomNodeSyle.fontSize = 12;
        _roomNodeSyle.normal.textColor = Color.white;

        selectedRoomNodeStyle = new GUIStyle();
        selectedRoomNodeStyle.normal.background =
            EditorGUIUtility.Load("node1 on") as Texture2D;
        selectedRoomNodeStyle.normal.textColor = Color.white;
        selectedRoomNodeStyle.border =
            new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);
        selectedRoomNodeStyle.padding =
            new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable() {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void InspectorSelectionChanged() {
        if (Selection.activeObject is RoomNodeGraphSO) {
            var graph = Selection.activeObject as RoomNodeGraphSO;
            if (graph != currentRoomNodeGraph && graph != null) {
                currentRoomNodeGraph = graph;
                GUI.changed = true;
            }
        }
    }

    private void OnGUI() {
        if (!currentRoomNodeGraph) {
            return;
        }

        DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
        DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);


        DrawDraggedLine();
        ProcessEvents(Event.current);
        DrawRoomConnections();
        DrawRoomNodes();
        DrawSelectionBox();
        if (GUI.changed) {
            Repaint();
        }
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity,
        Color gray) {
        int verticalGridCount = Mathf.CeilToInt((position.width + gridSize) /
                                                gridSize);
        int horizontalGridCount = Mathf.CeilToInt((position.height + gridSize) /
                                                  gridSize);

        Handles.color = new Color(gray.r, gray.g, gray.b, gridOpacity);
        graphOffset += graphDrag * 0.5f;
        Vector3 newOffset = new Vector3(graphOffset.x % gridSize,
            graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalGridCount; i++) {
            Handles.DrawLine(
                new Vector3(gridSize * i, -gridSize, 0) + newOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) +
                newOffset);
        }

        for (int i = 0; i < horizontalGridCount; i++) {
            Handles.DrawLine(
                new Vector3(-gridSize, gridSize * i, 0) + newOffset,
                new Vector3(position.width + gridSize, gridSize * i, 0f) +
                newOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawSelectionBox() {
        if (!currentRoomNodeGraph.isDraggingSelectionBox) {
            return;
        }

        // when start and end are too close do not draw the box
        if (Vector2.Distance(currentRoomNodeGraph.selectionBoxStartPosition,
                currentRoomNodeGraph.selectionBoxEndPosition) < 10f) {
            return;
        }

        // Draw four lines to create a box
        Vector2 topLeft = new Vector2(MathF.Min(
                currentRoomNodeGraph.selectionBoxStartPosition.x,
                currentRoomNodeGraph.selectionBoxEndPosition.x),
            MathF.Min(currentRoomNodeGraph.selectionBoxStartPosition.y,
                currentRoomNodeGraph.selectionBoxEndPosition.y));
        Vector2 bottomRight = new Vector2(MathF.Max(
                currentRoomNodeGraph.selectionBoxStartPosition.x,
                currentRoomNodeGraph.selectionBoxEndPosition.x),
            MathF.Max(currentRoomNodeGraph.selectionBoxStartPosition.y,
                currentRoomNodeGraph.selectionBoxEndPosition.y));
        Vector2 topRight = new Vector2(bottomRight.x, topLeft.y);
        Vector2 bottomLeft = new Vector2(topLeft.x, bottomRight.y);

        Handles.DrawBezier(topLeft, topRight, topLeft, topRight, Color.white,
            null, connectingLineWidth);
        Handles.DrawBezier(topRight, bottomRight, topRight, bottomRight,
            Color.white,
            null, connectingLineWidth);
        Handles.DrawBezier(bottomRight, bottomLeft, bottomRight, bottomLeft,
            Color.white, null, connectingLineWidth);
        Handles.DrawBezier(bottomLeft, topLeft, bottomLeft, topLeft,
            Color.white, null, connectingLineWidth);
    }

    private void DrawRoomConnections() {
        if (!currentRoomNodeGraph) {
            return;
        }

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            foreach (string childNodeID in roomNode.childNodes) {
                if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(
                        childNodeID)) {
                    RoomNodeSO childNode =
                        currentRoomNodeGraph.roomNodeDictionary[childNodeID];
                    DrawConnectionLine(roomNode, childNode);
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parent, RoomNodeSO child) {
        Vector2 startPosition = parent.rect.center;
        Vector2 endPosition = child.rect.center;

        // Draw a direction line from parent to child, show the array direction
        Vector2 middlePoint = startPosition + (endPosition - startPosition) / 2;
        Vector2 direction = endPosition - startPosition;

        Vector2 arrowTrailPoint1 =
            middlePoint - new Vector2(-direction.y, direction.x).normalized *
            connectingArrowWidth;
        Vector2 arrowTrailPoint2 =
            middlePoint + new Vector2(-direction.y, direction.x).normalized *
            connectingArrowWidth;

        Vector2 arrowHeadPoint =
            middlePoint + direction.normalized * connectingArrowWidth;

        Handles.DrawBezier(arrowHeadPoint, arrowTrailPoint1, arrowHeadPoint,
            arrowTrailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTrailPoint2, arrowHeadPoint,
            arrowTrailPoint2, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(startPosition, endPosition, startPosition,
            endPosition,
            Color.white, null, connectingLineWidth);
        GUI.changed = true;
    }


    private void DrawDraggedLine() {
        if (!currentRoomNodeGraph.roomNodeToDrawLineFrom ||
            currentRoomNodeGraph.linePosition == Vector2.zero) {
            return;
        }


        Handles.DrawBezier(
            currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
            currentRoomNodeGraph.linePosition,
            currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
            currentRoomNodeGraph.linePosition,
            Color.white, null, connectingLineWidth);
    }

    private void DrawRoomNodes() {
        if (!currentRoomNodeGraph) {
            return;
        }

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.Draw(roomNode.isSelected
                ? selectedRoomNodeStyle
                : _roomNodeSyle);
        }

        GUI.changed = true;
    }


    private void ProcessEvents(Event e) {
        graphDrag = Vector2.zero;
        if (!currentRoomNode || !currentRoomNode.isLeftClickDragging) {
            currentRoomNode = IsMouseOverRoomNode(e);
        }

        if (currentRoomNodeGraph.roomNodeToDrawLineFrom ||
            !currentRoomNode || currentRoomNodeGraph.isDraggingSelectionBox) {
            ProcessRoomNodeGraphEvents(e);
        } else {
            currentRoomNode.ProcessEvents(e);
        }
    }

    private void ProcessRoomNodeGraphEvents(Event e) {
        switch (e.type) {
            case EventType.MouseDown:
                ProcessMouseDownEvent(e);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(e);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(e);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent) {
        if (currentEvent.button == 1 &&
            currentRoomNodeGraph.roomNodeToDrawLineFrom) {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if (roomNode &&
                roomNode != currentRoomNodeGraph.roomNodeToDrawLineFrom) {
                AutoFillMiddleCorridor(
                    currentRoomNodeGraph.roomNodeToDrawLineFrom, roomNode);
                ClearLineDrag();
            } else if (!roomNode) {
                ShowContextMenuAddNew(currentEvent.mousePosition);
            }
        } else if (currentEvent.button == 0) {
            currentRoomNodeGraph.selectionBoxEndPosition =
                currentEvent.mousePosition;
            foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
                if (currentRoomNodeGraph.IsWithinSelectionBounds(roomNode)) {
                    roomNode.isSelected = true;
                }
            }

            currentRoomNodeGraph.isDraggingSelectionBox = false;
        }
    }

    struct RoomNodeTypeMenuItem
    {
        public Vector2 mousePosition;
        public RoomNodeTypeSO roomNodeType;
    }

    private void ShowContextMenuAddNew(Vector2 currentEventMousePosition) {
        GenericMenu genericMenu = new GenericMenu();
        foreach (var roomNodeType in roomNodeTypeList.list) {
            if (!roomNodeType.displayInNodeGraphEditor) {
                continue;
            }

            genericMenu.AddItem(new GUIContent("Add " + roomNodeType
                    .roomNodeTypeName + " Node"),
                false, CreateRoomNodeAndLink, new RoomNodeTypeMenuItem {
                    mousePosition = currentEventMousePosition,
                    roomNodeType = roomNodeType
                });
        }

        genericMenu.ShowAsContext();
    }

    private void CreateRoomNodeAndLink(object userdata) {
        if (userdata is RoomNodeTypeMenuItem) {
            RoomNodeTypeMenuItem roomNodeTypeMenuItem =
                (RoomNodeTypeMenuItem)userdata;
            RoomNodeSO roomNode = CreateRoomNode(
                roomNodeTypeMenuItem.mousePosition,
                roomNodeTypeMenuItem.roomNodeType);

            if (roomNode) {
                AutoFillMiddleCorridor(
                    currentRoomNodeGraph.roomNodeToDrawLineFrom, roomNode);
            }
        }

        ClearLineDrag();
    }

    private void AutoFillMiddleCorridor(RoomNodeSO parent, RoomNodeSO child) {
        RoomNodeTypeSO middleCorridorType = roomNodeTypeList.list.Find(x =>
            x.isCorridor && x.displayInNodeGraphEditor);
        if (parent.roomNodeType.isCorridor ||
            child.roomNodeType.isCorridor) {
            SetParentAndChildNode(parent, child);
            return;
        }


        Vector2 midPositionParentChild =
            parent.rect.center + (child.rect.center - parent.rect.center) / 2;
        Vector2 offsetForNodeWidthAndHeight =
            new Vector2(NodeWidth, NodeHeight) /
            2;
        midPositionParentChild -= offsetForNodeWidthAndHeight;
        RoomNodeSO middleCorridor = CreateRoomNode(
            midPositionParentChild,
            middleCorridorType);
        if (middleCorridor) {
            SetParentAndChildNode(parent, middleCorridor);
            SetParentAndChildNode(middleCorridor, child);
        } else {
            SetParentAndChildNode(parent, child);
        }
    }

    private void SetParentAndChildNode(RoomNodeSO parent, RoomNodeSO child) {
        if (parent.AddChildNodeIDToRoomNode(child.id)) {
            child.AddParentNodeIDToRoomNode(parent.id);
        }
    }

    private void ClearLineDrag() {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ProcessMouseDragEvent(Event currentEvent) {
        if (currentEvent.button == 1) {
            ProcessRightMouseDragEvent(currentEvent);
        } else if (currentEvent.button == 0) {
            ProcessLeftMouseDragEvent(currentEvent);
        } else if (currentEvent.button == 2) {
            ProcessMiddleMouseDragEvent(currentEvent);
        }
    }

    private void ProcessMiddleMouseDragEvent(Event currentEvent) {
        Vector2 delta = currentEvent.delta;
        graphDrag = delta;
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.DragNode(delta);
        }

        GUI.changed = true;
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent) {
        // Draw the selection box visual
        if (currentRoomNodeGraph.isDraggingSelectionBox) {
            currentRoomNodeGraph.selectionBoxEndPosition =
                currentEvent.mousePosition;
            GUI.changed = true;
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent) {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom) {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta) {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ProcessMouseDownEvent(Event currentEvent) {
        if (currentEvent.button == 1) {
            if (currentRoomNodeGraph.roomNodeToDrawLineFrom) {
                ClearLineDrag();
            }

            ShowContextMenu(currentEvent.mousePosition);
        } else if (currentEvent.button == 0) {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
            currentRoomNodeGraph.selectionBoxStartPosition =
                currentEvent.mousePosition;
            currentRoomNodeGraph.selectionBoxEndPosition =
                currentEvent.mousePosition;
            currentRoomNodeGraph.isDraggingSelectionBox = true;
        }
    }

    private void ClearAllSelectedRoomNodes() {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.isSelected = false;
            GUI.changed = true;
        }
    }


    private void ShowContextMenu(Vector2 mousePosition) {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, CreateRoomNode,
            mousePosition);
        genericMenu.AddSeparator("");
        genericMenu.AddItem(new GUIContent("Select All Node"), false,
            SelectAllNodes);
        genericMenu.AddSeparator("");
        genericMenu.AddItem(
            new GUIContent("Delete Links between Selected Nodes"),
            false, DeleteSelectedRoomNodeLink);
        genericMenu.AddItem(
            new GUIContent("Delete Relative Links of Selected Nodes"),
            false, DeleteSelectedRoomNodeRelativeLink);
        genericMenu.AddItem(new GUIContent("Delete Selected Room Nodes"),
            false, DeleteSelectedRoomNodes);

        genericMenu.ShowAsContext();
    }

    private void DeleteSelectedRoomNodeRelativeLink() {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
            if (roomNode.isSelected) {
                roomNode.ClearAllRelatedConnection();
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodes() {
        List<RoomNodeSO> roomNodesToDelete = new List<RoomNodeSO>();
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance) {
                roomNodesToDelete.Add(roomNode);
                roomNode.ClearAllRelatedConnection();
            }
        }

        foreach (var roomNode in roomNodesToDelete) {
            currentRoomNodeGraph.roomNodeList.Remove(roomNode);
            //remove from dictionary
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNode.id);
            DestroyImmediate(roomNode, true);
            AssetDatabase.SaveAssets();
        }

        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodeLink() {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
            if (roomNode.isSelected) {
                roomNode.ClearConnectionBetweenSelectedNode();
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void SelectAllNodes() {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.isSelected = true;
            GUI.changed = true;
        }
    }


    private void CreateRoomNode(object mousePositionObject) {
        if (currentRoomNodeGraph.roomNodeList.Count == 0) {
            CreateRoomNode(new Vector2(200f, 200f),
                roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject,
            roomNodeTypeList.list[0].roomNodeTypeName == "None"
                ? roomNodeTypeList.list[0]
                : roomNodeTypeList.list.Find(x => x.isNone));
    }

    private RoomNodeSO CreateRoomNode(object mousePositionObject,
        RoomNodeTypeSO roomNodeType) {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        if (!currentRoomNodeGraph) {
            return null;
        }

        RoomNodeSO roomNode = CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialize(
            new Rect(mousePosition, new Vector2(NodeWidth, NodeHeight)),
            currentRoomNodeGraph, roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.OnValidate();
        return roomNode;
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent) {
        if (!currentRoomNodeGraph) {
            return null;
        }

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            if (roomNode.rect.Contains(currentEvent.mousePosition)) {
                return roomNode;
            }
        }

        return null;
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line) {
        RoomNodeGraphSO roomNodeGraph =
            EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null) {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }

    [MenuItem("Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
}
