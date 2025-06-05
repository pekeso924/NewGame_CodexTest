using NUnit.Framework;
using UnityEngine;

public class GameTestsEdit
{
    [Test]
    public void ReflectHorizontal()
    {
        Vector3 dir = new Vector3(1, 0, 0);
        Vector3 normal = Vector3.left;
        Vector3 reflected = ReflectionUtility.Reflect(dir, normal);
        Assert.AreEqual(new Vector3(-1, 0, 0), reflected);
    }

    [Test]
    public void ReflectVertical()
    {
        Vector3 dir = new Vector3(0, 1, 0);
        Vector3 normal = Vector3.down;
        Vector3 reflected = ReflectionUtility.Reflect(dir, normal);
        Assert.AreEqual(new Vector3(0, -1, 0), reflected);
    }

    [Test]
    public void ReflectDiagonal()
    {
        Vector3 dir = (new Vector3(1, -1, 0)).normalized;
        Vector3 normal = Vector3.up;
        Vector3 expected = (new Vector3(1, 1, 0)).normalized;
        Vector3 reflected = ReflectionUtility.Reflect(dir, normal);
        Assert.AreEqual(0f, Vector3.Angle(expected, reflected), 0.0001f);
    }

    [Test]
    public void ArenaRotates()
    {
        GameObject go = new GameObject();
        var arena = go.AddComponent<ArenaController>();
        arena.Rotate(45f);
        Assert.AreEqual(45f, go.transform.eulerAngles.z, 0.1f);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void ScoreIncrements()
    {
        GameObject gmObj = new GameObject();
        var gm = gmObj.AddComponent<GameManager>();
        gm.TargetHit(null);
        Assert.AreEqual(1, gm.score);
        Object.DestroyImmediate(gmObj);
    }
}
