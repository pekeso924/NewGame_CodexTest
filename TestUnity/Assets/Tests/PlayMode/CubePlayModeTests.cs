#if UNITY_INCLUDE_TESTS
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CubePlayModeTests
{
    private GameObject go;
    private TestScript script;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        go = new GameObject("TestObject");
        script = go.AddComponent<TestScript>();
        yield return null; // wait a frame for Start()
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(go);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CubeMovesLeftRightAndParticlePlays()
    {
        Assert.IsNotNull(script.cube);
        Assert.IsNotNull(script.particle);

        float startX = script.cube.transform.position.x;
        Assert.IsTrue(script.particle.isPlaying);

        yield return new WaitForSeconds(0.5f);
        float rightX = script.cube.transform.position.x;
        Assert.Greater(rightX, startX);

        yield return new WaitForSeconds(1.0f);
        float leftX = script.cube.transform.position.x;
        Assert.Less(leftX, startX);

        yield return new WaitForSeconds(0.5f);
        float endX = script.cube.transform.position.x;
        Assert.AreEqual(startX, endX, 0.1f);

        // Wait for particle to finish (duration is 1s)
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(script.particle.isPlaying);
    }
}
#endif
