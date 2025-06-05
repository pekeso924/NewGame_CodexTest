using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction = Vector2.up;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch()
    {
        direction = Vector2.up;
    }

    public static Vector2 ReflectVector(Vector2 dir, Vector2 normal)
    {
        return Vector2.Reflect(dir, normal);
    }

    void FixedUpdate()
    {
        rb.velocity = direction.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            direction = ReflectVector(direction, normal);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var target = other.GetComponent<Target>();
        if (target != null)
        {
            target.Hit();
        }
    }
}
