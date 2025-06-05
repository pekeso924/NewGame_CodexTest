using UnityEngine;

public class Target : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddScore();
            Destroy(gameObject);
        }
    }
}
