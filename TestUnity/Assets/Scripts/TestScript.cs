using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        // Create a cube and move it up slightly
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1, 0);
        Debug.Log("Cube created and positioned");
    }
}
