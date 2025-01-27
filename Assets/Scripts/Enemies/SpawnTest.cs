using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour {
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> enemySpawnableObjects;
    private RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject;
    private List<GameObject> instantiatedEnemies = new List<GameObject>();

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs) {
        if (instantiatedEnemies != null && instantiatedEnemies.Count > 0) {
            foreach (GameObject enemy in instantiatedEnemies) {
                Destroy(enemy);
            }

            instantiatedEnemies.Clear();
        }

        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplate == null) {
            return;
        }

        enemySpawnableObjects = roomTemplate.enemiesByLevelList;

        randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(enemySpawnableObjects);
    }

    private void Start() {
        randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(enemySpawnableObjects);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            EnemyDetailsSO enemyDetails = randomSpawnableObject.GetItem();
            if (enemyDetails == null) {
                return;
            }

            instantiatedEnemies.Add(Instantiate(enemyDetails.enemyPrefab, HelperUtilities.GetSpawnPointNearestToPlayer
                    (HelperUtilities.GetMouseWorldPosition()),
                Quaternion.identity));
        }
    }
}
