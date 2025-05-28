using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject cube;
    public ParticleSystem particle;
    public bool movementFinished = false;
    public int loops = 2;
    public float halfCycleDuration = 0.2f;
    public float amplitude = 1f;

    void Start()
    {
        // Create cube at left position
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(-amplitude, 1f, 0f);

        // Add a particle system to the cube
        particle = cube.AddComponent<ParticleSystem>();
        var main = particle.main;
        main.loop = false;
        particle.Play();

        // Start movement coroutine
        StartCoroutine(MoveCube());
    }

    IEnumerator MoveCube()
    {
        for (int i = 0; i < loops; i++)
        {
            yield return MoveSegment(-amplitude, amplitude);
            yield return MoveSegment(amplitude, -amplitude);
        }
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        movementFinished = true;
    }

    IEnumerator MoveSegment(float startX, float endX)
    {
        float t = 0f;
        while (t < halfCycleDuration)
        {
            float progress = t / halfCycleDuration;
            float x = Mathf.Lerp(startX, endX, progress);
            cube.transform.position = new Vector3(x, 1f, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        cube.transform.position = new Vector3(endX, 1f, 0f);
    }
}
