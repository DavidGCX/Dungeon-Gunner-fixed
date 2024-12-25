using System.Collections;
using UnityEngine;

public static class HelperUtilities {
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName,
        string fieldValue) {
        if (string.IsNullOrEmpty(fieldValue)) {
            Debug.LogError("Field " + fieldName + " is empty in " + thisObject.name);
            return true;
        }

        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck) {
        if (objectToCheck == null) {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }

        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName,
        IEnumerable enumerableObjectToCheck) {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null) {
            Debug.LogError("Field " + fieldName + " is null in " + thisObject.name);
            return true;
        }

        foreach (var item in enumerableObjectToCheck) {
            if (item == null) {
                Debug.LogError("Field " + fieldName + " has a null value at index " +
                               count +
                               " in " + thisObject.name);
                error = true;
            } else {
                count++;
            }
        }

        if (count == 0) {
            Debug.LogError("Field " + fieldName + " is empty in " + thisObject.name);
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck,
        bool isZeroAllowed) {
        bool error = false;
        if (isZeroAllowed) {
            if (valueToCheck < 0) {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        } else {
            if (valueToCheck <= 0) {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static Vector3 GetSpawnPointNearestToPlayer(Vector3 playerPosition) {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPoint = new Vector3(10000f, 10000f, 0f);
        foreach (var spawnPoint in currentRoom.spawnPositionArray) {
            Vector3 spawnPointWorldPosition = grid.CellToWorld(new Vector3Int(spawnPoint.x, spawnPoint.y, 0));
            if (Vector3.Distance(playerPosition, spawnPointWorldPosition) <
                Vector3.Distance(playerPosition, nearestSpawnPoint)) {
                nearestSpawnPoint = spawnPointWorldPosition;
            }
        }

        return nearestSpawnPoint;
    }
}
