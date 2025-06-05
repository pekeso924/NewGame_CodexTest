using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GameTests
{
    [Test]
    public void GameManagerCreatesArena()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        var method = typeof(GameManager).GetMethod("CreateArena", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(gm, null);
        Assert.IsNotNull(gm.arenaRoot);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void SpawnTargetsCreatesSpecifiedNumber()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.targetCount = 5;
        var spawn = typeof(GameManager).GetMethod("SpawnTargets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        spawn.Invoke(gm, null);
        var targets = GameObject.FindObjectsOfType<Target>();
        Assert.AreEqual(5, targets.Length);
        Object.DestroyImmediate(go);
        foreach (var t in targets) Object.DestroyImmediate(t.gameObject);
    }

    [Test]
    public void AddScoreIncrements()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.AddScore();
        var scoreField = typeof(GameManager).GetField("score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        int score = (int)scoreField.GetValue(gm);
        Assert.AreEqual(1, score);
        Object.DestroyImmediate(go);
    }
}
