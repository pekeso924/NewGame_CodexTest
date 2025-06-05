using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GamePlayTests
{
    [UnityTest]
    public IEnumerator GameNotEndedAtStart()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        yield return null;
        Assert.IsFalse(gm.GameEnded);
    }

    [UnityTest]
    public IEnumerator BallExistsOnStart()
    {
        var gmObj = new GameObject();
        var gm = gmObj.AddComponent<GameManager>();
        gm.SetupScene();
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<Ball>());
    }

    [UnityTest]
    public IEnumerator BallMovesConstantSpeed()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        var ball = gm.BallInstance;
        yield return new WaitForFixedUpdate();
        float speed = ball.GetComponent<Rigidbody2D>().velocity.magnitude;
        Assert.AreEqual(ball.speed, speed, 0.01f);
    }

    [UnityTest]
    public IEnumerator BallReflectsOnWall()
    {
        Vector2 reflected = Ball.ReflectVector(Vector2.right, Vector2.left);
        Assert.AreEqual(Vector2.left, reflected);
        yield break;
    }

    [UnityTest]
    public IEnumerator DestroyTargetIncrementsScore()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.targetCount = 1;
        gm.SetupScene();
        var target = gm.Targets[0];
        gm.BallInstance.transform.position = target.transform.position;
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(1, gm.Score);
        Assert.IsTrue(gm.Targets.Count == 0);
    }

    [UnityTest]
    public IEnumerator GameClearsWhenTargetsGone()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.targetCount = 1;
        gm.SetupScene();
        gm.BallInstance.transform.position = gm.Targets[0].transform.position;
        yield return new WaitForFixedUpdate();
        yield return null;
        Assert.IsTrue(gm.GameEnded);
    }

    [UnityTest]
    public IEnumerator GameEndsOnTimeout()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.timeLimit = 0.1f;
        gm.SetupScene();
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(gm.GameEnded);
    }

    [UnityTest]
    public IEnumerator WallRotationChangesAngle()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        var rotator = GameObject.Find("Playfield").GetComponent<WallRotator>();
        float initial = rotator.transform.eulerAngles.z;
        rotator.ApplyRotation(1f, 1f);
        yield return null;
        Assert.Greater(rotator.transform.eulerAngles.z, initial);
    }

    [UnityTest]
    public IEnumerator TimerCountsDown()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.timeLimit = 1f;
        gm.SetupScene();
        float start = gm.TimeRemaining;
        yield return new WaitForSeconds(0.5f);
        Assert.Less(gm.TimeRemaining, start);
    }

    [UnityTest]
    public IEnumerator BallSpeedUnchangedAfterBounce()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        var ball = gm.BallInstance;
        ball.transform.position = new Vector2(gm.arenaSize/2 - 0.1f, 0);
        yield return new WaitForFixedUpdate();
        Vector2 dir = (Vector2)typeof(Ball).GetField("direction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(ball);
        float mag = dir.magnitude;
        Assert.AreEqual(1f, mag, 0.01f);
    }
}
