using UnityEngine;

public class WallRotator : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        ApplyRotation(input);
    }

    public void ApplyRotation(float direction, float deltaTime = -1f)
    {
        if (deltaTime < 0f) deltaTime = Time.deltaTime;
        transform.Rotate(0, 0, -direction * rotationSpeed * deltaTime);
    }
}
