using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections;

public class GamePlayTests
{
    [UnityTest]
    public IEnumerator SceneLoadsWithGameManager()
    {
        SceneManager.LoadScene("GeneratedScene");
        yield return null;
        var gm = Object.FindObjectOfType<GameManager>();
        Assert.IsNotNull(gm);
    }
}
