using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManagerSpawner
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            new GameObject("GameManager", typeof(GameManager));
        }
    }
}
