using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T> {
    private struct chanceBoundaries {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList) {
        this.spawnableObjectByLevelList = spawnableObjectByLevelList;
    }

    public T GetItem() {
        int upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectByLevel<T> spawnableObjectByLevel in spawnableObjectByLevelList) {
            if (spawnableObjectByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel()) {
                foreach (var spawnableObjectRatio in spawnableObjectByLevel.spawnableObjectRatios) {
                    int lowerBoundary = upperBoundary + 1;
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;
                    ratioValueTotal += spawnableObjectRatio.ratio;
                    chanceBoundariesList.Add(new chanceBoundaries {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        if (chanceBoundariesList.Count == 0) {
            return default(T);
        }

        int lookUpValue = Random.Range(0, ratioValueTotal);

        foreach (var chanceBoundary in chanceBoundariesList) {
            if (lookUpValue >= chanceBoundary.lowBoundaryValue && lookUpValue <= chanceBoundary.highBoundaryValue) {
                spawnableObject = chanceBoundary.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
