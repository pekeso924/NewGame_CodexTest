using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CommandLine
{
    // Method to create a new scene and add a GameObject with TestScript
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject go = new GameObject("TestObject");
        go.AddComponent<TestScript>();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GeneratedScene.unity");
        Debug.Log("Scene generated and saved");
    }
}
