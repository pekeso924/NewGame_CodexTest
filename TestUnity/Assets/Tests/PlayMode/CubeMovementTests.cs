using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CubeMovementTests
{
    [UnityTest]
    public IEnumerator CubeMovesLeftRightAndParticlesPlay()
    {
        var go = new GameObject("Cube");
        var mover = go.AddComponent<CubeMover>();
        mover.frequency = 1f;
        mover.amplitude = 1f;

        // wait a frame so CubeMover.Start adds the ParticleSystem
        yield return null;
        var ps = go.GetComponent<ParticleSystem>();
        Vector3 startPos = go.transform.position;
        yield return new WaitForSeconds(0.6f);
        Vector3 midPos = go.transform.position;
        yield return new WaitForSeconds(0.6f);
        Vector3 endPos = go.transform.position;

        Assert.AreNotEqual(startPos.x, midPos.x);
        Assert.AreNotEqual(midPos.x, endPos.x);
        Assert.IsTrue(Mathf.Sign(midPos.x) != Mathf.Sign(endPos.x));
        Assert.IsTrue(ps.isPlaying);
    }
}
