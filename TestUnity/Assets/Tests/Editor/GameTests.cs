using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameTests
{
    [Test]
    public void GameSceneContainsManager()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        var gm = Object.FindObjectOfType<GameManager>();
        Assert.IsNotNull(gm);
    }

    [Test]
    public void SetupArenaCreatesFourWalls()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.SetupArena();
        Assert.AreEqual(4, gm.arenaRoot.transform.childCount);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void SpawnBallCreatesBallWithComponents()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.SpawnBall();
        Assert.IsNotNull(gm.ball.GetComponent<Rigidbody2D>());
        Assert.IsNotNull(gm.ball.GetComponent<CircleCollider2D>());
        Object.DestroyImmediate(gm.ball);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void SpawnTargetsCreatesSpecifiedCount()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.SpawnTargets(3);
        Assert.AreEqual(3, gm.targets.Count);
        foreach (var t in gm.targets) Object.DestroyImmediate(t);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void TargetDestroyedIncrementsScore()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        var target = new GameObject();
        gm.targets.Add(target);
        gm.TargetDestroyed(target);
        Assert.AreEqual(1, gm.score);
        Object.DestroyImmediate(go);
    }
}
