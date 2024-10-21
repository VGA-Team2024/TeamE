using UnityEditor;
using UnityEngine;
public static class MissingScriptRemover
{
    [MenuItem("MyTools/Remove Missing Scripts")] 
    private static void RemoveMissingScripts()
    {
        int missingCount = 0;
        foreach (var obj in Selection.gameObjects)
        {
            missingCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
        }
        Debug.Log($"{missingCount} 個のmissingScriptを取り除きました");
    }
}