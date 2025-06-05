using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public float rotationSpeed = 60f;
    public float testInputOverride = 0f;
    void Update()
    {
        float h = testInputOverride != 0f ? testInputOverride : Input.GetAxisRaw("Horizontal");
        transform.Rotate(0,0,-h * rotationSpeed * Time.deltaTime);
    }
}
