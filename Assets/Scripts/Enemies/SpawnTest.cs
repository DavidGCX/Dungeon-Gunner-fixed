using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour {
    public RoomTemplateSO roomTemplate;
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> enemySpawnableObjects;
    private RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject;
    private GameObject enemySpawned;

    private void Start() {
        enemySpawnableObjects = roomTemplate.enemiesByLevelList;
        randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(enemySpawnableObjects);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            if (enemySpawned != null) {
                Destroy(enemySpawned);
            }

            EnemyDetailsSO enemyDetails = randomSpawnableObject.GetItem();
            if (enemyDetails == null) {
                return;
            }

            enemySpawned = Instantiate(enemyDetails.enemyPrefab, HelperUtilities.GetSpawnPointNearestToPlayer
                    (HelperUtilities.GetMouseWorldPosition()),
                Quaternion.identity);
        }
    }
}
