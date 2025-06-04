using UnityEngine;

public class Target : MonoBehaviour
{
    public GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallMarker>())
        {
            manager.RegisterHit(this);
            Destroy(gameObject);
        }
    }
}
