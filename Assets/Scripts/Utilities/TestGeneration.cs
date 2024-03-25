using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class TestGeneration : MonoBehaviour {
    // Start is called before the first frame update

    [ButtonInvoke(nameof(GenerateMap))] public bool OutputTheMapGenerated;

    [ButtonInvoke(nameof(DeleteGeneratedMap))]
    public bool DeleteTheMapGenerated;

    [ContextMenu("Generate Map And Save To File")]
    private void GenerateMap() {
        RoomNodeGraphSO roomNodeGraphSO =
            ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        // save the asset to the path
        roomNodeGraphSO.GenerateEntrance();
        roomNodeGraphSO.GenerateDungeonGraphTest();
        roomNodeGraphSO.Awake();
        Debug.Log("Loading complete");
    }

    public void DeleteGeneratedMap() {
        string path = "Assets/Resources/RoomNodeGraphSO.asset";
        AssetDatabase.DeleteAsset(path);
    }
}