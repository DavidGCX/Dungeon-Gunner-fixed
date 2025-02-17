using System.Collections;
using UnityEngine;

public static class HelperUtilities {
    public static Camera mainCamera;

    public static Vector3 GetMouseWorldPosition() {
        if (!mainCamera) {
            mainCamera = Camera.main;
        }

        if (!mainCamera) {
            Debug.LogError("Main camera not found in the scene");
            return Vector3.zero;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        // clamp to screen size only
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0, Screen.height);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;
        return mouseWorldPosition;
    }
    public static Vector3 GetMouseWorldPosition(Vector3 mouseScreenPosition) {
        if (!mainCamera) {
            mainCamera = Camera.main;
        }

        if (!mainCamera) {
            Debug.LogError("Main camera not found in the scene");
            return Vector3.zero;
        }

        // clamp to screen size only
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0, Screen.height);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;
        return mouseWorldPosition;
    }

    public static float GetAngleFromVector(Vector3 vector) {
        vector = vector.normalized;
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    public static AimDirection GetAimDirection(float angle) {
        if (angle is >= 22f and <= 67f) {
            return AimDirection.UpRight;
        } else if (angle is > 67f and < 112f) {
            return AimDirection.Up;
        } else if (angle is >= 112f and <= 158f) {
            return AimDirection.UpLeft;
        } else if (angle is (> 158f and <= 180) or (>= -180f and < -135f)) {
            return AimDirection.Left;
        } else if (angle is >= -135f and <= -45f) {
            return AimDirection.Down;
        } else {
            return AimDirection.Right;
        }
    }

    public static Vector3 GetDirectionVectorFromAngle(float angle) {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
    }

    public static AimDirection GetAimDirection(Vector3 direction) {
        float angle = GetAngleFromVector(direction);
        if (angle is >= 22f and <= 67f) {
            return AimDirection.UpRight;
        } else if (angle is > 67f and < 112f) {
            return AimDirection.Up;
        } else if (angle is >= 112f and <= 158f) {
            return AimDirection.UpLeft;
        } else if (angle is (> 158f and <= 180) or (>= -180f and < -135f)) {
            return AimDirection.Left;
        } else if (angle is >= -135f and <= -45f) {
            return AimDirection.Down;
        } else {
            return AimDirection.Right;
        }
    }

    public static float LinearToDecibels(float linear) {
        float linearScaleRange = 20f;

        return Mathf.Log10(linear / linearScaleRange) * 20f;
    }

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

    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck,
        bool isZeroAllowed) {
        bool error = false;
        if (isZeroAllowed) {
            if (valueToCheck < 0f) {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        } else {
            if (valueToCheck <= 0f) {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float
        valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed) {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum) {
            Debug.Log(fieldNameMinimum + " must be less than " + fieldNameMaximum + " in object " +
                      thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) {
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) {
            error = true;
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
