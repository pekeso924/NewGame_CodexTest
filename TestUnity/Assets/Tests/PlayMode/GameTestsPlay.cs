using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameTestsPlay
{
    private const string SceneName = "ManagerScene";

    [UnityTest]
    public IEnumerator ObjectsSpawned()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        Assert.IsNotNull(Object.FindObjectOfType<GameManager>());
        Assert.IsNotNull(Object.FindObjectOfType<BulletController>());
        Assert.Greater(Object.FindObjectsOfType<Target>().Length, 0);
    }

    [UnityTest]
    public IEnumerator BulletMoves()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        var bullet = Object.FindObjectOfType<BulletController>();
        Vector3 start = bullet.transform.position;
        yield return new WaitForSeconds(0.5f);
        Vector3 end = bullet.transform.position;
        Assert.Greater(Vector3.Distance(start, end), 0.1f);
    }

    [UnityTest]
    public IEnumerator TargetDestroyedOnHit()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        var gm = Object.FindObjectOfType<GameManager>();
        var target = Object.FindObjectOfType<Target>();
        int initial = gm.score;
        target.Hit();
        yield return null;
        Assert.AreEqual(initial + 1, gm.score);
    }

    [UnityTest]
    public IEnumerator TimeUpEndsGame()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        var gm = Object.FindObjectOfType<GameManager>();
        gm.timeLimit = 0.5f;
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(gm.enabled);
    }

    [UnityTest]
    public IEnumerator ClearEndsGame()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        var gm = Object.FindObjectOfType<GameManager>();
        foreach (var t in Object.FindObjectsOfType<Target>())
        {
            t.Hit();
        }
        yield return null;
        Assert.IsFalse(gm.enabled);
    }
}
