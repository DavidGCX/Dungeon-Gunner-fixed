using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

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

    [Space(10)] [Header("PLAYER")] public CurrentPlayerSO currentPlayer;

    [Space(10)] [Header("SOUNDS")] public AudioMixerGroup soundsMasterMixerGroup;
    public SoundEffectSO doorOpenSoundEffect;
    [Space(10)] [Header("MATERIALS")] public Material dimmedMaterial;

    public Material litMaterial;

    public Shader variableLitShader;

    [Space(10)] [Header("Special tilemap tiles for testing")]
    public TileBase[] enemyUnwalkableCollisionTileArray;

    public TileBase preferredEnemyPathTile;

    [Space(10)] [Header("UI")] public GameObject ammoIconPrefab;

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenSoundEffect), doorOpenSoundEffect);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTileArray),
            enemyUnwalkableCollisionTileArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
    }
#endif

    #endregion
}
