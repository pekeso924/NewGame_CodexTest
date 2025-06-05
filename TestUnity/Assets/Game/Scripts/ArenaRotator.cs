using UnityEngine;

public class ArenaRotator : MonoBehaviour
{
    public float rotationSpeed = 60f;

    /// <summary>
    /// Rotate the arena by input amount for given delta time.
    /// </summary>
    /// <param name="input">-1 to 1 value.</param>
    /// <param name="deltaTime">Delta time in seconds.</param>
    public void ApplyRotation(float input, float deltaTime)
    {
        transform.Rotate(Vector3.forward, rotationSpeed * input * deltaTime, Space.World);
    }

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        ApplyRotation(input, Time.deltaTime);
    }
}
