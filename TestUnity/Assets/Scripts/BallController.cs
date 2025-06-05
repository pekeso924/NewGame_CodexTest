using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rb;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.right * speed;
    }

    void Start()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude != speed)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<TargetController>();
        if (target != null)
        {
            target.manager.TargetHit(other.gameObject);
        }
    }
}
