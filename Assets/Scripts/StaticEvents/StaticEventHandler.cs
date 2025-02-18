using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticEventHandler {
    // room changed event
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room) {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs { room = room });
    }
    public static event Action<RoomEnemiesDefeatedEventArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room) {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedEventArgs { room = room });
    }
}

public class RoomChangedEventArgs : EventArgs {
    public Room room;
}

public class RoomEnemiesDefeatedEventArgs : EventArgs {
    public Room room;
}
