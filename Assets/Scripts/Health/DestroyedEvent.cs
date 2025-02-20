using UnityEngine;
using System;


[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour {
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool isPlayerDied, int points) {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs() { isPlayerDied = isPlayerDied, points = points });
    }
}

public class DestroyedEventArgs : EventArgs {
    public bool isPlayerDied;
    public int points;
}
