using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        // Create a cube and add movement with particles
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1, 0);
        cube.AddComponent<CubeMover>();
        Debug.Log("Cube created with movement and particles");
    }
}
