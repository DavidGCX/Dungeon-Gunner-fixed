using UnityEngine;
using System;

public class Node : IComparable<Node> {
    public Vector2Int gridPosition;
    public Node parentNode;

    public int fCost {
        get { return gCost + hCost; }
    }

    public int gCost = 0;
    public int hCost = 0;

    public Node(Vector2Int gridPosition, Node parentNode = null) {
        this.gridPosition = gridPosition;
        this.parentNode = parentNode;
    }

    public int CompareTo(Node other) {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(other.hCost);
        }

        return compare;
    }
}
