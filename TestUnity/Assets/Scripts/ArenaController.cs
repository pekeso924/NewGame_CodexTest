using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public float rotateSpeed = 60f;

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        transform.Rotate(0f, input * rotateSpeed * Time.deltaTime, 0f);
    }
}
