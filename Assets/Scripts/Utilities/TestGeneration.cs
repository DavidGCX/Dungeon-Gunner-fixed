using UnityEngine;


[ExecuteInEditMode]
public class TestGeneration : MonoBehaviour {
    // Start is called before the first frame update

    [ButtonInvoke(nameof(MyFunction))] public bool OutputTheMapGenerated;

    [ContextMenu("Generate Map And Save To File")]
    private void MyFunction() {
        RoomNodeGraphSO roomNodeGraphSO =
            ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        string path = "Assets/Resources/RoomNodeGraphSO.asset";
        UnityEditor.AssetDatabase.CreateAsset(roomNodeGraphSO, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        roomNodeGraphSO.GenerateEntrance();
    }
}