using UnityEngine;

public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }
}
