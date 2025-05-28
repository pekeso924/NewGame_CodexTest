using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class CubeMovementTests
{
    [UnityTest]
    public IEnumerator CubeMovesAndParticlePlays()
    {
        var go = new GameObject();
        var script = go.AddComponent<TestScript>();

        // Wait until cube is created
        yield return new WaitUntil(() => script.cube != null);

        // Wait until cube reaches right side
        yield return new WaitUntil(() => script.cube.transform.position.x > 0.9f);
        Assert.IsTrue(script.particle.isPlaying, "Particle should be playing while moving");

        // Wait until cube reaches left side
        yield return new WaitUntil(() => script.cube.transform.position.x < -0.9f);
        Assert.IsTrue(script.particle.isPlaying, "Particle should still be playing");

        // Wait for movement to finish
        yield return new WaitUntil(() => script.movementFinished);
        Assert.IsFalse(script.particle.isPlaying, "Particle should stop after movement ends");
    }
}
