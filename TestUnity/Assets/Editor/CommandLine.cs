using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CommandLine
{
    // Method to create a new scene containing a GameManager root
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GeneratedScene.unity");
        Debug.Log("Scene generated and saved");
    }
}
