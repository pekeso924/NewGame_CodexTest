using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GamePlayTests
{
    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        BallController.freeze = true;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        yield return null; // wait one frame for scene load
        yield return null; // extra frame to ensure Start has run
        var ball = Object.FindObjectOfType<BallController>();
        if (ball != null)
            ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        BallController.freeze = false;
        yield return null;
    }

    [UnityTest]
    public IEnumerator BallExistsAtStart()
    {
        yield return null;
        Assert.IsNotNull(Object.FindObjectOfType<BallController>());
    }

    [UnityTest]
    public IEnumerator TargetsSpawnedAtStart()
    {
        yield return null;
        var gm = Object.FindObjectOfType<GameManager>();
        Assert.AreEqual(gm.targetCount, gm.targets.Count);
    }

    [UnityTest]
    public IEnumerator DestroyTargetIncrementsScore()
    {
        var gm = Object.FindObjectOfType<GameManager>();
        var target = gm.targets[0];
        gm.TargetDestroyed(target);
        Object.Destroy(target);
        yield return null;
        Assert.AreEqual(gm.targetCount - 1, gm.targets.Count);
        Assert.AreEqual(1, gm.score);
    }

    [UnityTest]
    public IEnumerator RotateArenaChangesRotation()
    {
        var gm = Object.FindObjectOfType<GameManager>();
        var arena = gm.arenaRoot.GetComponent<ArenaController>();
        float before = gm.arenaRoot.transform.rotation.eulerAngles.z;
        arena.testInputOverride = 1f;
        yield return new WaitForSeconds(0.1f);
        arena.testInputOverride = 0f;
        float after = gm.arenaRoot.transform.rotation.eulerAngles.z;
        Assert.AreNotEqual(before, after);
    }

    [UnityTest]
    public IEnumerator GameClearsWhenAllTargetsDestroyed()
    {
        var gm = Object.FindObjectOfType<GameManager>();
        foreach (var t in new System.Collections.Generic.List<GameObject>(gm.targets))
        {
            gm.TargetDestroyed(t);
            Object.Destroy(t);
        }
        yield return null;
        Assert.AreEqual(GameManager.GameState.Cleared, gm.state);
    }
}
