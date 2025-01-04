using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class TestGeneration : MonoBehaviour {
#if UNITY_EDITOR
    // Start is called before the first frame update

    private RoomNodeGraphSO currentGraph;

    public List<DungeonLevelRestriction> dungeonLevelRestrictions =
        new List<DungeonLevelRestriction>();

    [Range(0, 5)] public int DungeonLevel = 0;

    [ButtonInvoke(nameof(GenerateMap))] public bool OutputTheMapGenerated;

    [ButtonInvoke(nameof(SavedGeneratedMap))]
    public bool SaveTheMapGenerated;

    [ButtonInvoke(nameof(DeleteGeneratedMap))]
    public bool DeleteTheMapGenerated;

    [ContextMenu("Generate Map And Save To File")]
    private void GenerateMap() {
        currentGraph =
            ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        string path = "Assets/Resources/RoomNodeGraphSO.asset";
        AssetDatabase.CreateAsset(currentGraph, path);
        // save the asset to the path
        currentGraph.GenerateEntrance(saveToAsset: true);
        currentGraph.GenerateDungeonGraphTest(
            dungeonLevelRestrictions[
                Mathf.Clamp(DungeonLevel, 0, dungeonLevelRestrictions.Count - 1)],
            saveToAsset: true);
        currentGraph.LoadRoomNodeDictionary();
        Debug.Log("Loading complete");
    }

    public void SavedGeneratedMap() {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void DeleteGeneratedMap() {
        string path = "Assets/Resources/RoomNodeGraphSO.asset";
        AssetDatabase.DeleteAsset(path);
    }
#endif
}
