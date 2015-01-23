using UnityEditor;
using UnityEngine;

public class PrefabEditorMenu {
    [MenuItem ("Tools/Revert to Prefab %r")]
    public static void Revert() {
        GameObject[] selection = Selection.gameObjects;
        if(selection.Length > 0) {
            for (int i = 0; i < selection.Length; i++) {
                // EditorUtility.ResetGameObjectToPrefabState(selection[i]);
                PrefabUtility.RevertPrefabInstance(selection[i]);
            }
        }
        else {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }
}