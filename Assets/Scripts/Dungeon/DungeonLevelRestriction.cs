using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelRestriction",
    menuName = "NodeGraph/DungeonLevelRestriction")]
public class DungeonLevelRestriction : ScriptableObject {
    // TODO: change to list and enum to make it more flexible
    [Header("Small Room")] public int minSmallRoomCount = 0;
    public int maxSmallRoomCount = 0;
    [Header("Medium Room")] [Space(10)] public int minMediumRoomCount = 0;
    public int maxMediumRoomCount = 0;
    [Header("Large Room")] [Space(10)] public int minLargeRoomCount = 0;
    public int maxLargeRoomCount = 0;

    [Header("Boss Room")]
    [Space(10)]
    [Tooltip(
        "Please keep min and max boss room count the same for now. Will work on more later")]
    public int minBossRoomCount = 1;

    public int maxBossRoomCount = 1;
    [Header("Chest Room")] [Space(10)] public int minChestRoomCount = 0;
    public int maxChestRoomCount = 0;
}