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
        gm.initialTargetCount = 3;
        gm.Initialize();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gm.gameObject);
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            if (obj != null && obj.name != "GameManager")
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    [Test]
    public void CreatesBall()
    {
        Assert.IsNotNull(GameObject.Find("Ball"));
    }

    [Test]
    public void CreatesFourWalls()
    {
        Assert.AreEqual(4, gm.arena.transform.childCount);
    }

    [Test]
    public void CreatesTargets()
    {
        Assert.AreEqual(3, gm.targets.Count);
    }

    [Test]
    public void TargetHitIncrementsScore()
    {
        var target = gm.targets[0];
        gm.TargetHit(target);
        Assert.AreEqual(1, gm.score);
        Assert.AreEqual(2, gm.targets.Count);
    }

    [Test]
    public void RotateArenaChangesAngle()
    {
        float initial = gm.arena.transform.eulerAngles.z;
        gm.RotateArena(1f, 1f);
        Assert.AreNotEqual(initial, gm.arena.transform.eulerAngles.z);
    }
}
