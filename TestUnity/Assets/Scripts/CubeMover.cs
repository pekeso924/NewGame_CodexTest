using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f; // cycles per second

    private Vector3 startPos;
    private ParticleSystem particles;

    void Start()
    {
        startPos = transform.position;
        particles = gameObject.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.loop = true;
        particles.Play();
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * 2f * Mathf.PI * frequency) * amplitude;
        transform.position = startPos + Vector3.right * x;
    }
}
