using NUnit.Framework;
using UnityEngine;

public class GameEditTests
{
    [SetUp]
    public void Init()
    {
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            if (obj.hideFlags == HideFlags.None)
                Object.DestroyImmediate(obj);
        }
    }
    [Test]
    public void SetupCreatesPlayfield()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.SetupScene();
        Assert.IsNotNull(GameObject.Find("Playfield"));
        Object.DestroyImmediate(go);
    }

    [Test]
    public void CreatesFourWalls()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GameManager>();
        gm.SetupScene();
        int count = 0;
        foreach (var col in GameObject.FindObjectsOfType<BoxCollider2D>())
        {
            if (col.gameObject.name == "Wall") count++;
        }
        Assert.AreEqual(4, count);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void CreatesBall()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        Assert.IsNotNull(GameObject.FindObjectOfType<Ball>());
    }

    [Test]
    public void CreatesTargets()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.targetCount = 3;
        gm.SetupScene();
        Assert.AreEqual(3, GameObject.FindObjectsOfType<Target>().Length);
    }

    [Test]
    public void BallReflectsHorizontally()
    {
        Vector2 reflected = Ball.ReflectVector(Vector2.right, Vector2.left);
        Assert.AreEqual(Vector2.left, reflected);
    }

    [Test]
    public void BallReflectsVertically()
    {
        Vector2 reflected = Ball.ReflectVector(Vector2.up, Vector2.down);
        Assert.AreEqual(Vector2.down, reflected);
    }

    [Test]
    public void WallRotatorHasDefaultSpeed()
    {
        var wr = new GameObject().AddComponent<WallRotator>();
        Assert.AreEqual(90f, wr.rotationSpeed);
    }

    [Test]
    public void GameManagerScoreStartsZero()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.SetupScene();
        Assert.AreEqual(0, gm.Score);
    }

    [Test]
    public void TimeLimitDefaultThirty()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        Assert.AreEqual(30f, gm.timeLimit);
    }

    [Test]
    public void TargetsHaveComponent()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        gm.targetCount = 2;
        gm.SetupScene();
        foreach (var t in GameObject.FindObjectsOfType<Target>())
        {
            Assert.IsNotNull(t.GetComponent<Target>());
        }
    }
}
