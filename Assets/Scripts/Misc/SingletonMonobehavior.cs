using System;
using UnityEngine;

public abstract class SingletonMonobehavior<T> : MonoBehaviour
    where T : MonoBehaviour {
    private static T instance;

    public static T Instance {
        get { return instance; }
    }

    protected virtual void Awake() {
        if (instance == null) {
            instance = this as T;
            // This will cause some reference errors when reloading scenes
            // So disable For now until a better solution is found for it
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
