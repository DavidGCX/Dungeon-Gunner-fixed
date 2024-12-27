using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: ALL the event should be simplified to using Unity Input System later on in the project

[DisallowMultipleComponent]
public class IdleEvent : MonoBehaviour {
    public event Action<IdleEvent> OnIdle;

    public void CallIdleEvent() {
        OnIdle?.Invoke(this);
    }
}
