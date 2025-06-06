using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerPlayTests
{
    GameManager gm;
    Target dummy;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        var go = new GameObject();
        gm = go.AddComponent<GameManager>();
        gm.targetCount = 0;
        gm.playTime = 0.2f;
        gm.wallSize = 4f;
        gm.Initialize();
        var obj = new GameObject();
        dummy = obj.AddComponent<Target>();
        obj.AddComponent<CircleCollider2D>().isTrigger = true;
        dummy.manager = gm;
        dummy.transform.position = Vector3.left * 10f;
        gm.targets.Add(dummy);
        yield return null; // wait one frame
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(gm.wallsParent.gameObject);
        Object.Destroy(gm.gameObject);
        foreach (var t in gm.targets)
            Object.Destroy(t.gameObject);
        if (dummy) Object.Destroy(dummy.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator BallMoves()
    {
        Vector3 start = gm.ball.transform.position;
        yield return new WaitForSeconds(0.1f);
        Assert.AreNotEqual(start, gm.ball.transform.position);
    }

    [UnityTest]
    public IEnumerator ScoreIncrementsOnHit()
    {
        var tgtObj = new GameObject();
        var target = tgtObj.AddComponent<Target>();
        tgtObj.AddComponent<CircleCollider2D>().isTrigger = true;
        target.manager = gm;
        target.transform.position = gm.ball.transform.position + Vector3.right * 0.5f;
        gm.targets.Add(target);
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(1, gm.Score);
    }

    [UnityTest]
    public IEnumerator ClearAfterAllTargetsDestroyed()
    {
        gm.targets.Remove(dummy);
        Object.Destroy(dummy.gameObject);
        var tgtObj = new GameObject();
        var target = tgtObj.AddComponent<Target>();
        tgtObj.AddComponent<CircleCollider2D>().isTrigger = true;
        target.manager = gm;
        gm.targets.Add(target);
        gm.TargetHit(target);
        yield return null;
        Assert.AreEqual(GameManager.GameState.Clear, gm.State);
    }

    [UnityTest]
    public IEnumerator TimeUpEndsGame()
    {
        gm.playTime = 0.1f;
        var tgtObj = new GameObject();
        var target = tgtObj.AddComponent<Target>();
        tgtObj.AddComponent<CircleCollider2D>().isTrigger = true;
        target.manager = gm;
        target.transform.position = Vector3.left * 10f;
        gm.targets.Add(target);
        yield return new WaitForSeconds(0.15f);
        Assert.AreEqual(GameManager.GameState.TimeUp, gm.State);
    }

    [UnityTest]
    public IEnumerator RotateWallsAffectsDirection()
    {
        float before = gm.wallsParent.eulerAngles.z;
        gm.RotateWalls(45f);
        yield return null;
        float after = gm.wallsParent.eulerAngles.z;
        Assert.AreNotEqual(before, after);
    }
}
