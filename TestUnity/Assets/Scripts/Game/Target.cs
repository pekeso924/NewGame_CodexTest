using UnityEngine;

public class Target : MonoBehaviour
{
    public GameManager Manager { get; set; }

    public void Hit()
    {
        Manager.TargetDestroyed(this);
        Destroy(gameObject);
    }
}
