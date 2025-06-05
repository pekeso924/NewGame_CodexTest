using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float timeLimit = 30f;
    public int score = 0;
    public float elapsed = 0f;

    public GameObject bulletPrefab;
    public GameObject targetPrefab;

    private GameObject bulletInstance;
    private List<Target> targets = new List<Target>();
    private ArenaController arena;

    void Start()
    {
        arena = GetComponent<ArenaController>();
        if (arena == null)
        {
            arena = gameObject.AddComponent<ArenaController>();
        }
        SetupArena();
        SpawnBullet();
        SpawnTargets();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= timeLimit)
        {
            EndGame(false);
        }
        if (targets.Count == 0 && elapsed < timeLimit)
        {
            EndGame(true);
        }
    }

    public void TargetHit(Target target)
    {
        score++;
        targets.Remove(target);
    }

    public void HitTarget(GameObject targetGo)
    {
        var t = targetGo.GetComponent<Target>();
        if (t != null)
        {
            TargetHit(t);
        }
    }

    void EndGame(bool cleared)
    {
        Debug.Log($"Game Over {(cleared ? "CLEAR" : "TIME UP")}, Score:{score}, Time:{elapsed:F2}");
        Destroy(bulletInstance);
        enabled = false;
    }

    void SetupArena()
    {
        // create child object for arena walls rotated 45 degrees
        transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        float size = 10f;
        float thickness = 0.5f;
        CreateWall(new Vector3(-size/2, 0f, 0f), new Vector3(thickness, 1f, size));
        CreateWall(new Vector3(size/2, 0f, 0f), new Vector3(thickness, 1f, size));
        CreateWall(new Vector3(0f, 0f, -size/2), new Vector3(size, 1f, thickness));
        CreateWall(new Vector3(0f, 0f, size/2), new Vector3(size, 1f, thickness));
    }

    void CreateWall(Vector3 pos, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.tag = "Wall";
        wall.transform.SetParent(transform, false);
        wall.transform.localPosition = pos;
        wall.transform.localScale = scale;
        var collider = wall.GetComponent<BoxCollider>();
        collider.material = new PhysicMaterial { bounciness = 1f, bounceCombine = PhysicMaterialCombine.Maximum, frictionCombine = PhysicMaterialCombine.Minimum };
    }

    void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            bulletInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulletInstance.name = "Bullet";
            bulletInstance.transform.position = Vector3.zero;
            bulletInstance.AddComponent<Rigidbody>();
            bulletInstance.AddComponent<BulletController>();
        }
        else
        {
            bulletInstance = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    void SpawnTargets()
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(3,0,0), new Vector3(-3,0,0), new Vector3(0,0,3), new Vector3(0,0,-3)
        };
        foreach (var pos in positions)
        {
            GameObject go;
            if (targetPrefab == null)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale = Vector3.one * 0.5f;
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null) renderer.material.color = new Color(1f,0.5f,0f);
                go.AddComponent<Rigidbody>().isKinematic = true;
                go.AddComponent<Target>();
            }
            else
            {
                go = Instantiate(targetPrefab, pos, Quaternion.identity);
            }
            go.name = "Target";
            go.tag = "Target";
            go.transform.position = pos;
            targets.Add(go.GetComponent<Target>());
        }
    }

    public int TargetCount => targets.Count;
}
