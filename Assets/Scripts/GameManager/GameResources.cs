using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour {
    private static GameResources instance;

    public static GameResources Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<GameResources>("GameResources");
            }

            return instance;
        }
    }

    [Space(10)]
    [Header("DUNGEON GENERATION")]
    [Tooltip(
        "Should be populate with the sripable object that contains the list of all the node type for the game, it is used instead of enum")]
    public RoomNodeTypeListSO roomNodeTypeList;


    public Material dimmedMaterial;
}
