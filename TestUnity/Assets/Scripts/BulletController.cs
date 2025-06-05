using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = transform.forward.normalized * speed;
    }

    void FixedUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.GetContact(0).normal;
        rb.velocity = ReflectionUtility.Reflect(rb.velocity.normalized, normal) * speed;

        if (collision.gameObject.CompareTag("Target"))
        {
            FindObjectOfType<GameManager>()?.HitTarget(collision.gameObject);
        }
    }
}
