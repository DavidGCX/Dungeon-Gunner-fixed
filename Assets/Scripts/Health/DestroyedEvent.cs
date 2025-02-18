using UnityEngine;
using System;


[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool isPlayerDied)
    {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs() { isPlayerDied = isPlayerDied });
    }
}

public class DestroyedEventArgs : EventArgs {
    public bool isPlayerDied;
}
