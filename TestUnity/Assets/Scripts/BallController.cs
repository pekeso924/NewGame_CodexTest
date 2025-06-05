using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody2D rb;
    public static bool freeze = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!freeze)
            rb.velocity = new Vector2(1, 0).normalized * speed;
        else
            rb.velocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (freeze)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = rb.velocity.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponent<Target>())
        {
            var gm = FindObjectOfType<GameManager>();
            gm.TargetDestroyed(col.collider.gameObject);
            Destroy(col.collider.gameObject);
        }
    }
}
