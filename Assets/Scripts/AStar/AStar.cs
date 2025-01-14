using UnityEngine;
using System.Collections.Generic;

public static class AStar {
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition) {
        // Get (0, 0) based grid positions
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();
        GridNodes gridNodes = new GridNodes(room.templateWidth, room.templateHeight);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);
        if (startNode == null || targetNode == null) {
            return null;
        }

        Node endPathNode = FindShortestPath(gridNodes, openNodeList, closedNodeHashSet, startNode, targetNode,
            room.instantiatedRoom);

        if (endPathNode == null) {
            Debug.LogWarning("AStar: No path found at room " + room.id + " from " + startGridPosition + " to " +
                             endGridPosition);
            return null;
        } else {
            return CreatePathStack(endPathNode, room);
        }
    }

    private static Node FindShortestPath(GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet,
        Node startNode, Node targetNode, InstantiatedRoom instantiatedRoom) {
        openNodeList.Add(startNode);
        while (openNodeList.Count > 0) {
            openNodeList.Sort();
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            if (currentNode == targetNode) {
                return currentNode;
            }

            closedNodeHashSet.Add(currentNode);
            EvaluateCurrentNodeNeighbours(gridNodes, openNodeList, closedNodeHashSet, currentNode, targetNode,
                instantiatedRoom);
        }

        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, Node currentNode, Node targetNode, InstantiatedRoom instantiatedRoom) {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;
        Node validNeighbourNode;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + x, currentNodeGridPosition.y + y,
                    gridNodes, closedNodeHashSet, instantiatedRoom);
                if (validNeighbourNode != null) {
                    int movementPenalty = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                        validNeighbourNode.gridPosition.y];
                    var newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) +
                                             movementPenalty;
                    bool isValidNeighbourInOpenList = openNodeList.Contains(validNeighbourNode);
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourInOpenList) {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;
                        if (!isValidNeighbourInOpenList) {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node currentNode, Node validNeighbourNode) {
        int xDistance = Mathf.Abs(currentNode.gridPosition.x - validNeighbourNode.gridPosition.x);
        int yDistance = Mathf.Abs(currentNode.gridPosition.y - validNeighbourNode.gridPosition.y);
        if (xDistance > yDistance) {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        } else {
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
    }

    private static Node GetValidNodeNeighbour(int x, int y, GridNodes gridNodes, HashSet<Node> closedNodeHashSet,
        InstantiatedRoom instantiatedRoom) {
        if (x >= instantiatedRoom.room.templateWidth || x < 0 || y >= instantiatedRoom.room.templateHeight || y < 0) {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(x, y);
        int movementPenalty = instantiatedRoom.aStarMovementPenalty[x, y];

        if (movementPenalty == 0 || closedNodeHashSet.Contains(neighbourNode)) {
            return null;
        } else {
            return neighbourNode;
        }
    }

    private static Stack<Vector3> CreatePathStack(Node endPathNode, Room room) {
        Stack<Vector3> pathStack = new Stack<Vector3>();
        Node nextNode = endPathNode;

        Vector3 cellMidpoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidpoint.z = 0f;
        while (nextNode != null) {
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x +
                room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));
            worldPosition += cellMidpoint;
            pathStack.Push(worldPosition);
            nextNode = nextNode.parentNode;
        }

        return pathStack;
    }
}
