using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehavior<PoolManager> {
    [System.Serializable]
    public class Pool {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new SerializedDictionary<int, Queue<Component>>();

    private void Start() {
        objectPoolTransform = this.gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++) {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize, string componentType) {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey)) {
            poolDictionary.Add(poolKey, new Queue<Component>());
            for (int i = 0; i < poolSize; i++) {
                GameObject gameObject = Instantiate(prefab, parentGameObject.transform);
                gameObject.SetActive(false);
                poolDictionary[poolKey].Enqueue(gameObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation) {
        int poolKey = prefab.GetInstanceID();
        if (poolDictionary.ContainsKey(poolKey) && poolDictionary[poolKey].Count > 0) {
            Component componentToReuse = GetComponentFromPool(poolKey);
            ResetObject(position, rotation, componentToReuse, prefab);
            return componentToReuse;
        } else {
            Debug.LogWarning("Pool for " + prefab + " does not exist.");
            return null;
        }
    }

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab) {
        var transformComponent = componentToReuse.transform;
        transformComponent.position = position;
        transformComponent.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    private Component GetComponentFromPool(int poolKey) {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);
        if (componentToReuse.gameObject.activeSelf) {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif

    #endregion
}
