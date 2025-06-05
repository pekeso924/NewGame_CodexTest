using NUnit.Framework;
using UnityEngine;

public class GameTests
{
    [Test]
    public void ArenaRotatesPositive()
    {
        var go = new GameObject();
        var rotator = go.AddComponent<ArenaRotator>();
        rotator.rotationSpeed = 60f;
        rotator.ApplyRotation(1f, 1f);
        Assert.AreEqual(60f, go.transform.eulerAngles.z, 0.1f);
    }

    [Test]
    public void ArenaRotatesNegative()
    {
        var go = new GameObject();
        var rotator = go.AddComponent<ArenaRotator>();
        rotator.rotationSpeed = 60f;
        rotator.ApplyRotation(-1f, 1f);
        float angle = go.transform.eulerAngles.z;
        if (angle > 180f) angle -= 360f;
        Assert.AreEqual(-60f, angle, 0.1f);
    }

    [Test]
    public void TimerCountsDown()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.StartGame();
        gc.Tick(1f);
        Assert.Less(gc.TimeRemaining, gc.timeLimit);
    }

    [Test]
    public void GameEndsWhenTimeUp()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.timeLimit = 1f;
        gc.StartGame();
        gc.Tick(1.1f);
        Assert.IsTrue(gc.GameOver);
    }

    [Test]
    public void ScoreIncrementsOnTargetDestroyed()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.StartGame();
        var t = gc.targetPrefab;
        gc.TargetDestroyed(t);
        Assert.AreEqual(1, gc.Score);
    }

    [Test]
    public void GameEndsWhenAllTargetsGone()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.targetCount = 1;
        gc.StartGame();
        var target = GameObject.FindObjectOfType<Target>();
        gc.TargetDestroyed(target);
        gc.Tick(0.1f);
        Assert.IsTrue(gc.GameOver);
    }

    [Test]
    public void BallSpawnedAtOrigin()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.StartGame();
        Assert.NotNull(GameObject.FindObjectOfType<BallController>());
        Assert.AreEqual(Vector3.zero, GameObject.FindObjectOfType<BallController>().transform.position);
    }

    [Test]
    public void TargetsSpawnedCount()
    {
        var obj = new GameObject();
        var gc = obj.AddComponent<GameController>();
        gc.ballPrefab = new GameObject().AddComponent<BallController>();
        gc.targetPrefab = new GameObject().AddComponent<Target>();
        gc.targetCount = 20;
        gc.StartGame();
        Assert.AreEqual(20, gc.ActiveTargetCount);
    }

    [Test]
    public void BallHasHighBounciness()
    {
        var ball = new GameObject();
        var bc = ball.AddComponent<BallController>();
        bc.SetupPhysics();
        var col = ball.GetComponent<SphereCollider>();
        Assert.GreaterOrEqual(col.material.bounciness, 0.8f);
    }
}
