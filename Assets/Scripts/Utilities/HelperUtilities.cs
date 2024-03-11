using System.Collections;
using UnityEngine;

public static class HelperUtilities {
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName,
        string fieldValue) {
        if (string.IsNullOrEmpty(fieldValue)) {
            Debug.LogError("Field " + fieldName + " is empty in " + thisObject.name);
            return false;
        }

        return true;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName,
        IEnumerable enumerableObjectToCheck) {
        bool error = false;
        int count = 0;
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
}