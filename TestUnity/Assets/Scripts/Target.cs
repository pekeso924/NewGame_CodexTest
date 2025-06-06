using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector] public GameManager manager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() != null)
        {
            manager.TargetHit(this);
        }
    }
}
