using UnityEngine;

public class Target : MonoBehaviour
{
    public void Hit()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.TargetHit(this);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BulletController>() != null)
        {
            Hit();
        }
    }
}
