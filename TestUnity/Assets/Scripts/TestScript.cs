using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject cube;
    public ParticleSystem particle;

    void Start()
    {
        // Create a cube and position it slightly above ground
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1, 0);

        // Add a simple particle system to the cube
        particle = cube.AddComponent<ParticleSystem>();
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Configure particle system to loop while the cube exists
        var main = particle.main;
        main.duration = 1f;
        main.startLifetime = 0.5f;
        main.loop = true;

        // Ensure the particle uses a valid material so it is not shown as a purple square
        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        particle.Play();

        Debug.Log("Cube created and particle started");
    }

    void Update()
    {
        if (cube != null)
        {
            // Move cube left and right with a period of 2 seconds
            float x = Mathf.Sin(Time.time * Mathf.PI);
            cube.transform.position = new Vector3(x, 1, 0);
        }
    }
}
