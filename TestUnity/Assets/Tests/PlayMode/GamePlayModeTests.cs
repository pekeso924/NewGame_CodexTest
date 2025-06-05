using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GamePlayModeTests
{
    GameManager gm;

    [UnitySetUp]
    public IEnumerator SetUpScene()
    {
        var go = new GameObject("GameManager");
        gm = go.AddComponent<GameManager>();
        gm.initialTargetCount = 1;
        gm.timeLimit = 5f;
        gm.Initialize();
        yield return null; // wait one frame for Start
    }

    [UnityTearDown]
    public IEnumerator TearDownScene()
    {
        Object.Destroy(gm.gameObject);
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(obj);
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator BallExists()
    {
        yield return null;
        Assert.IsNotNull(Object.FindObjectOfType<BallController>());
    }

    [UnityTest]
    public IEnumerator TargetDestroyedOnHit()
    {
        Vector3 pos = gm.ball.transform.position + Vector3.right * 0.6f;
        var target = gm.targets[0];
        target.transform.position = pos;
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(0, gm.targets.Count);
        Assert.AreEqual(1, gm.score);
    }

    [UnityTest]
    public IEnumerator RotationChangesArenaAngle()
    {
        float angle = gm.arena.transform.eulerAngles.z;
        gm.RotateArena(1f, 0.5f);
        yield return null;
        Assert.AreNotEqual(angle, gm.arena.transform.eulerAngles.z);
    }

    [UnityTest]
    public IEnumerator TimeUpEndsGame()
    {
        gm.timeLimit = 0.1f;
        gm.timeLeft = gm.timeLimit;
        yield return new WaitForSeconds(0.2f);
        Assert.IsFalse(gm.enabled);
    }

    [UnityTest]
    public IEnumerator ClearEndsGame()
    {
        Vector3 pos = gm.ball.transform.position + Vector3.right * 0.6f;
        gm.targets[0].transform.position = pos;
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(gm.enabled);
    }
}
