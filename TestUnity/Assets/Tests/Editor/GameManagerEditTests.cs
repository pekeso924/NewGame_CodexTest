using NUnit.Framework;
using UnityEngine;

public class GameManagerEditTests
{
    GameManager gm;

    [SetUp]
    public void Setup()
    {
        var go = new GameObject();
        gm = go.AddComponent<GameManager>();
        gm.targetCount = 2;
        gm.Initialize();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gm.gameObject);
        if (gm.wallsParent) Object.DestroyImmediate(gm.wallsParent.gameObject);
        foreach (var t in gm.targets)
            if (t) Object.DestroyImmediate(t.gameObject);
    }

    [Test]
    public void WallsCreatedAndRotated()
    {
        Assert.AreEqual(4, gm.wallsParent.childCount);
        Assert.AreEqual(45f, gm.wallsParent.eulerAngles.z, 0.1f);
    }

    [Test]
    public void BallCreated()
    {
        Assert.IsNotNull(gm.ball);
        Assert.IsNotNull(gm.ball.GetComponent<Rigidbody2D>());
    }

    [Test]
    public void TargetsSpawned()
    {
        Assert.AreEqual(2, gm.targets.Count);
    }

    [Test]
    public void TargetHitIncrementsScore()
    {
        var t = gm.targets[0];
        gm.TargetHit(t);
        Assert.AreEqual(1, gm.Score);
        Assert.AreEqual(1, gm.targets.Count);
    }

    [Test]
    public void EndGameChangesState()
    {
        gm.EndGame(GameManager.GameState.Clear);
        Assert.AreNotEqual(GameManager.GameState.Playing, gm.State);
    }
}
