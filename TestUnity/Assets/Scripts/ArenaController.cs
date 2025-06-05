using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) > 0f)
            Rotate(h * rotationSpeed * Time.deltaTime);
    }

    public void Rotate(float degrees)
    {
        transform.Rotate(0f, 0f, degrees, Space.World);
    }
}
