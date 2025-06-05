using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private void Awake()
    {
        SetupPhysics();
    }

    /// <summary>
    /// Configure rigidbody and collider material. Exposed for tests.
    /// </summary>
    public void SetupPhysics()
    {
        var rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;

        var col = GetComponent<SphereCollider>();
        var mat = new PhysicMaterial
        {
            bounciness = 0.9f,
            dynamicFriction = 0f,
            staticFriction = 0f,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        col.material = mat;
    }
}
