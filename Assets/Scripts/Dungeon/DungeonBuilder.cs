using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehavior<DungeonBuilder> {
    public Dictionary<string, Room> dungeonBuilderRoomDictionary =
        new Dictionary<string, Room>();

    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary =
        new Dictionary<string, RoomTemplateSO>();

    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    protected override void Awake() {
        base.Awake();
        LoadRoomNodeTypeList();
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    private void LoadRoomNodeTypeList() {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel) {
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        LoadRoomTemplatesIntoDictionary();
        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;
        while (!dungeonBuildSuccessful && dungeonBuildAttempts <
               Settings.MaxDungeonBuildAttempts) {
            dungeonBuildAttempts++;
            RoomNodeGraphSO roomNodeGraph;
            if (GameManager.Instance.RandomGeneratingNodeGraph &&
                currentDungeonLevel.usingDungeonLevelRestriction) {
                roomNodeGraph =
                    GenerateRandomRoomNodeGraph(currentDungeonLevel
                        .dungeonLevelRestriction);
            } else {
                roomNodeGraph = SelectRandomRoomNodeGraph(
                    currentDungeonLevel.roomNodeGraphList);
            }

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;
            while (!dungeonBuildSuccessful &&
                   dungeonRebuildAttemptsForNodeGraph <
                   Settings.MaxDungeonRebuildAttemptsForRoomGraph) {
                ClearDungeon();
                dungeonRebuildAttemptsForNodeGraph++;
                dungeonBuildSuccessful =
                    AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful) {
                InstantiateRoomGameObjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    private RoomNodeGraphSO GenerateRandomRoomNodeGraph(
        DungeonLevelRestriction dungeonLevelRestriction) {
        RoomNodeGraphSO roomNodeGraph =
            ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        roomNodeGraph.GenerateEntrance(saveToAsset: false);
        roomNodeGraph.GenerateDungeonGraphTest(
            dungeonLevelRestriction,
            saveToAsset:
            false);
        roomNodeGraph.LoadRoomNodeDictionary();
        return roomNodeGraph;
    }

    private void InstantiateRoomGameObjects() {
        foreach (var keyValuePair in dungeonBuilderRoomDictionary) {
            Room room = keyValuePair.Value;
            // we do minus here because the room position is relative to the template
            // When Initialize is called, the room will have position in template
            // space, so we need to minus the template lower bounds to get the correct position
            // as (0, 0) since our doorway calculation also uses the room position excluding
            // the template lower bounds
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room
                    .templateLowerBounds.x,
                room.lowerBounds.y - room.templateLowerBounds.y, 0);
            GameObject roomGameObject = Instantiate(room.prefab, roomPosition,
                Quaternion.identity, transform);
            InstantiatedRoom instantiatedRoom =
                roomGameObject.GetComponentInChildren<InstantiatedRoom>();
            instantiatedRoom.room = room;
            instantiatedRoom.Initialize(roomGameObject);
            room.instantiatedRoom = instantiatedRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateGuid) {
        if (roomTemplateDictionary.ContainsKey(roomTemplateGuid)) {
            return roomTemplateDictionary[roomTemplateGuid];
        } else {
            Debug.LogError("RoomTemplateSO with guid " + roomTemplateGuid +
                           " not found in the dictionary");
            return null;
        }
    }

    public Room GetRoomByRoomID(string roomId) {
        if (dungeonBuilderRoomDictionary.ContainsKey(roomId)) {
            return dungeonBuilderRoomDictionary[roomId];
        } else {
            Debug.LogError("Room with id " + roomId +
                           " not found in the dictionary");
            return null;
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph) {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        RoomNodeSO entranceRoomNode = roomNodeGraph.GetRoomNode(
            roomNodeTypeList.GetRoomNodeTypeFromName("Entrance"));
        if (!entranceRoomNode) {
            Debug.LogError("No entrance found in the roomNodeGraph");
            return false;
        }

        openRoomNodeQueue.Enqueue(entranceRoomNode);
        bool noRoomOverlaps = true;
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(
            roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps) {
            return true;
        } else {
            return false;
        }
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph,
        Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps) {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps) {
            RoomNodeSO currentRoomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childNode in roomNodeGraph.GetChildRoomNodes(
                         currentRoomNode)) {
                openRoomNodeQueue.Enqueue(childNode);
            }

            if (currentRoomNode.roomNodeType.isEntrance) {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(
                    currentRoomNode.roomNodeType);
                Room room =
                    CreateRoomFromRoomTemplate(roomTemplate, currentRoomNode);
                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
            } else {
                Room parentRoom = dungeonBuilderRoomDictionary[
                    currentRoomNode.parentNodes[0]];
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(currentRoomNode,
                    parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode,
        Room parentRoom) {
        bool roomOverlaps = true;

        // Try all available and unconnected doorways in the parent room
        while (roomOverlaps) {
            List<Doorway> unconnectedAvalidableParentDoorways =
                GetUnconnectedAvalidableParentDoorways(parentRoom.doorwayList)
                    .ToList();
            if (unconnectedAvalidableParentDoorways.Count == 0) {
                return false;
            }

            // Randomly select one door way
            Doorway parentDoorway = unconnectedAvalidableParentDoorways[
                UnityEngine.Random.Range(0,
                    unconnectedAvalidableParentDoorways.Count)];
            RoomTemplateSO roomTemplate =
                GetRandomTempalteForRoomConsistentWithParentDoorway(
                    roomNode, parentDoorway);
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
            if (PlaceTheRoom(parentRoom, parentDoorway, room)) {
                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
                roomOverlaps = false;
            } else {
                roomOverlaps = true;
            }
        }

        return true;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway parentDoorway,
        Room room) {
        Doorway doorway = GetOppositeDoorway(parentDoorway, room.doorwayList);
        if (doorway == null) {
            parentDoorway.isUnavailable = true;
            return false;
        }
        // This gives the new position for the doorway of the parent room.

        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds +
            parentDoorway.position - parentRoom
                .templateLowerBounds;

        // Position offset, if this doorway is west, we want a (1, 0) offset to the east of parent doorway
        Vector2Int adjustment = Vector2Int.zero;
        switch (parentDoorway.orientation) {
            case Orientation.north:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }
        // First parentDoorwayPosition + adjustment gives the correct position for the doorway
        // of the child

        // Second, + room.templateLowerBounds -doorway.position
        // gives the correct position for the lower bounds of the room
        room.lowerBounds = parentDoorwayPosition + adjustment + room
            .templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds -
                           room.templateLowerBounds;
        Room overlappingRoom = CheckForRoomOverlap(room);
        if (overlappingRoom == null) {
            parentDoorway.isConnected = true;
            parentDoorway.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;
            return true;
        } else {
            parentDoorway.isUnavailable = true;
            return false;
        }
    }

    private Room CheckForRoomOverlap(Room room) {
        foreach (var keyValuePair in dungeonBuilderRoomDictionary) {
            Room otherRoom = keyValuePair.Value;
            if (room.id == otherRoom.id || !otherRoom.isPositioned) {
                continue;
            }

            if (IsOverlappingRoom(room, otherRoom)) {
                return otherRoom;
            }
        }

        return null;
    }

    private bool IsOverlappingRoom(Room roomToTest, Room otherRoom) {
        return IsOverlappingInterval(roomToTest.lowerBounds.x,
                   roomToTest.upperBounds.x, otherRoom.lowerBounds.x,
                   otherRoom.upperBounds.x) &&
               IsOverlappingInterval(roomToTest.lowerBounds.y,
                   roomToTest.upperBounds.y, otherRoom.lowerBounds.y,
                   otherRoom.upperBounds.y);
    }

    private bool IsOverlappingInterval(int imin1, int imax1, int imin2,
        int imax2) {
        return Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2);
    }

    private Doorway GetOppositeDoorway(Doorway parentDoorway,
        List<Doorway> roomDoorwayList) {
        foreach (Doorway doorway in roomDoorwayList) {
            if (parentDoorway.orientation == Orientation.north &&
                doorway.orientation == Orientation.south) {
                return doorway;
            } else if (parentDoorway.orientation == Orientation.south &&
                       doorway.orientation == Orientation.north) {
                return doorway;
            } else if (parentDoorway.orientation == Orientation.east &&
                       doorway.orientation == Orientation.west) {
                return doorway;
            } else if (parentDoorway.orientation == Orientation.west &&
                       doorway.orientation == Orientation.east) {
                return doorway;
            }
        }

        return null;
    }

    private RoomTemplateSO GetRandomTempalteForRoomConsistentWithParentDoorway(
        RoomNodeSO roomNode, Doorway parentDoorway) {
        RoomTemplateSO roomTemplate = null;
        if (roomNode.roomNodeType.isCorridor) {
            switch (parentDoorway.orientation) {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(
                        roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(
                        roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                case Orientation.none:
                    break;
                default:
                    break;
            }
        } else {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    private IEnumerable<Doorway> GetUnconnectedAvalidableParentDoorways(
        List<Doorway> parentRoomDoorwayList) {
        foreach (Doorway doorway in parentRoomDoorwayList) {
            if (!doorway.isConnected && !doorway.isUnavailable) {
                yield return doorway;
            }
        }
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate,
        RoomNodeSO currentRoomNode) {
        Room room = new Room();
        room.templateID = roomTemplate.guid;
        room.roomNodeType = currentRoomNode.roomNodeType;
        room.id = currentRoomNode.id;
        room.prefab = roomTemplate.prefab;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.childRoomIDList = CopyStringList(currentRoomNode.childNodes);
        room.doorwayList = CopyDoorWayList(roomTemplate.doorwayList);

        if (currentRoomNode.parentNodes.Count == 0) {
            // This is the entrance room
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
        } else {
            room.parentRoomID = currentRoomNode.parentNodes[0];
        }

        return room;
    }

    private List<Doorway>
        CopyDoorWayList(List<Doorway> roomTemplateDoorwayList) {
        List<Doorway> copiedList = new List<Doorway>();
        foreach (Doorway doorway in roomTemplateDoorwayList) {
            Doorway newDoorway = new Doorway();
            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition =
                doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            copiedList.Add(newDoorway);
        }

        return copiedList;
    }

    private List<string> CopyStringList(List<string> childNodes) {
        List<String> copiedList = new List<string>();
        foreach (string childNode in childNodes) {
            copiedList.Add(childNode);
        }

        return copiedList;
    }

    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType) {
        List<RoomTemplateSO> matchingRoomTemplateList =
            new List<RoomTemplateSO>();
        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {
            if (roomTemplate.roomNodeType == roomNodeType) {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0) {
            Debug.LogError("No room templates found for roomNodeType " +
                           roomNodeType.roomNodeTypeName);
            return null;
        }

        return matchingRoomTemplateList[
            UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private void ClearDungeon() {
        if (dungeonBuilderRoomDictionary.Count > 0) {
            foreach (var keyValuePair in dungeonBuilderRoomDictionary) {
                Room room = keyValuePair.Value;
                if (room.instantiatedRoom) {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(
        List<RoomNodeGraphSO> roomNodeGraphList) {
        if (roomNodeGraphList.Count == 0) {
            Debug.LogError("No roomNodeGraphs found in the list");
            return null;
        }

        return roomNodeGraphList[
            UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
    }

    private void LoadRoomTemplatesIntoDictionary() {
        roomTemplateDictionary.Clear();
        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid)) {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            } else {
                Debug.LogError("RoomTemplateSO with guid " + roomTemplate.guid +
                               " already exists in the dictionary");
            }
        }
    }
}