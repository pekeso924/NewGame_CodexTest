using NUnit.Framework;
using UnityEngine;

public class GameManagerTests
{
    GameManager CreateManager(int targetCount = 3, float timeLimit = 30f)
    {
        var go = new GameObject("GM");
        var gm = go.AddComponent<GameManager>();
        gm.targetCount = targetCount;
        gm.timeLimit = timeLimit;
        gm.scoreText = new GameObject("score").AddComponent<SimpleText>();
        gm.timeText = new GameObject("time").AddComponent<SimpleText>();
        gm.ballPrefab = new GameObject("ball");
        gm.ballPrefab.AddComponent<BallMarker>();
        gm.ballPrefab.AddComponent<SphereCollider>();
        gm.ballPrefab.AddComponent<Rigidbody>();
        gm.ballPrefab.SetActive(false);
        gm.targetPrefab = new GameObject("target");
        gm.targetPrefab.AddComponent<Target>();
        gm.targetPrefab.AddComponent<SphereCollider>().isTrigger = true;
        gm.targetPrefab.GetComponent<Target>().manager = gm;
        gm.targetPrefab.SetActive(false);
        gm.Start();
        return gm;
    }

    [Test]
    public void CreatesArenaAndTargets()
    {
        var gm = CreateManager();
        Assert.IsNotNull(gm.arenaRoot);
        var list = (System.Collections.Generic.List<GameObject>)typeof(GameManager)
            .GetField("_targets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(gm);
        Assert.AreEqual(3, list.Count);
    }

    [Test]
    public void InitialScoreIsZero()
    {
        var gm = CreateManager();
        Assert.AreEqual("SCORE: 0", gm.scoreText.text);
    }

    [Test]
    public void RegisterHitIncrementsScore()
    {
        var gm = CreateManager();
        var target = Object.FindObjectOfType<Target>(true);
        gm.RegisterHit(target);
        Assert.AreEqual(1, gm.GetType().GetField("_score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gm));
    }

    [Test]
    public void GameEndsWhenTargetsGone()
    {
        var gm = CreateManager(1);
        var target = Object.FindObjectOfType<Target>(true);
        gm.RegisterHit(target);
        Assert.IsTrue(gm.GameOver);
    }

    [Test]
    public void TickAdvancesTime()
    {
        var gm = CreateManager();
        gm.Tick(1f, 0f);
        Assert.AreEqual(1f, gm.GetType().GetField("_time", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gm));
    }

    [Test]
    public void GameEndsWhenTimeLimitReached()
    {
        var gm = CreateManager(1, 0.5f);
        gm.Tick(1f, 0f);
        Assert.IsTrue(gm.GameOver);
    }

    [Test]
    public void RotateArenaChangesRotation()
    {
        var gm = CreateManager();
        var initial = gm.arenaRoot.rotation;
        gm.Tick(1f, 1f);
        Assert.AreNotEqual(initial, gm.arenaRoot.rotation);
    }

    [Test]
    public void ScoreTextUpdates()
    {
        var gm = CreateManager();
        var text = gm.scoreText;
        gm.RegisterHit(Object.FindObjectOfType<Target>(true));
        Assert.IsTrue(text.text.Contains("1"));
    }

    [Test]
    public void TimeTextUpdates()
    {
        var gm = CreateManager();
        gm.Tick(1f, 0f);
        Assert.IsTrue(gm.timeText.text.Contains("1.00"));
    }

    [Test]
    public void MultipleHitsIncreaseScore()
    {
        var gm = CreateManager(2);
        var targets = Object.FindObjectsOfType<Target>(true);
        foreach (var t in targets)
            gm.RegisterHit(t);
        Assert.AreEqual(2, gm.GetType().GetField("_score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gm));
    }
}
