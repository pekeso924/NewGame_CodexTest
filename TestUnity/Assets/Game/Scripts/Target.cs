using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Target : MonoBehaviour
{
    private void Reset()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallController>() != null)
        {
            var gc = FindObjectOfType<GameController>();
            if (gc != null)
                gc.TargetDestroyed(this);
            Destroy(gameObject);
        }
    }
}
