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
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}