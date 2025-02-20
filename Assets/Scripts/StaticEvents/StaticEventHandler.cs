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

    public static event Action<PointsScoredArgs> OnPointsScored;

    public static void CallPointsScoredEvent(int points) {
        OnPointsScored?.Invoke(new PointsScoredArgs { points = points });
    }

    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score) {
        OnScoreChanged?.Invoke(new ScoreChangedArgs { score = score });
    }

    public static event Action<MutipliersChangedArgs> OnMultipliersChanged;

    // Pass in the change of value not the value of the mutipliers
    public static void CallMultipliersChangedEvent(int multipliers) {
        OnMultipliersChanged?.Invoke(new MutipliersChangedArgs { mutipliers = multipliers });
    }
}

public class RoomChangedEventArgs : EventArgs {
    public Room room;
}

public class RoomEnemiesDefeatedEventArgs : EventArgs {
    public Room room;
}

public class PointsScoredArgs : EventArgs {
    public int points;
}

public class ScoreChangedArgs : EventArgs {
    public long score;
}

public class MutipliersChangedArgs : EventArgs {
    public int mutipliers;
}
