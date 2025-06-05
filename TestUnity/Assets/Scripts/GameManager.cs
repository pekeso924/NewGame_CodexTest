using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float playTime = 30f;
    public float elapsed;
    public GameObject ballPrefab;
    public GameObject targetPrefab;
    public int targetCount = 5;

    public List<GameObject> targets = new List<GameObject>();
    public GameObject arenaRoot;
    public GameObject ball;
    public int score;
    public enum GameState { Playing, Cleared, TimeUp }
    public GameState state = GameState.Playing;

    public void Start()
    {
        SetupArena();
        SpawnBall();
        SpawnTargets(targetCount);
    }

    void Update()
    {
        if (state != GameState.Playing) return;
        elapsed += Time.deltaTime;
        if (elapsed >= playTime)
        {
            state = GameState.TimeUp;
            Debug.Log($"Time Up! Score:{score}");
        }
        if (targets.Count == 0 && state == GameState.Playing)
        {
            state = GameState.Cleared;
            Debug.Log($"Cleared! Time:{elapsed:F1} Score:{score}");
        }
    }

    public void SetupArena()
    {
        if (arenaRoot == null)
        {
            arenaRoot = new GameObject("Arena");
            CreateWalls(arenaRoot.transform, 5f);
        }
    }

    void CreateWalls(Transform root, float halfSize)
    {
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        for (int i = 0; i < 4; i++)
        {
            var wall = new GameObject($"Wall{i}");
            wall.transform.SetParent(root);
            var col = wall.AddComponent<BoxCollider2D>();
            col.size = new Vector2(halfSize * 2, 0.2f);
            var rb = wall.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            wall.transform.localPosition = dirs[i] * halfSize;
            wall.transform.localRotation = Quaternion.Euler(0,0, i %2==0?0:90);
        }
        root.transform.rotation = Quaternion.Euler(0,0,45);
        root.gameObject.AddComponent<ArenaController>();
    }

    public void SpawnBall()
    {
        if (ballPrefab == null)
        {
            ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Ball";
#if UNITY_EDITOR
            Object.DestroyImmediate(ball.GetComponent<SphereCollider>());
#else
            Destroy(ball.GetComponent<SphereCollider>());
#endif
            var col = ball.AddComponent<CircleCollider2D>();
            var rb = ball.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            ball.AddComponent<BallController>();
        }
        else
        {
            ball = Instantiate(ballPrefab);
        }
    }

    public void SpawnTargets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 2f;
            var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            target.transform.position = pos;
#if UNITY_EDITOR
            Object.DestroyImmediate(target.GetComponent<SphereCollider>());
#else
            Destroy(target.GetComponent<SphereCollider>());
#endif
            target.AddComponent<CircleCollider2D>();
            target.AddComponent<Target>();
            targets.Add(target);
        }
    }

    public void TargetDestroyed(GameObject target)
    {
        targets.Remove(target);
        score++;
    }
}
