using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int targetCount = 3;
    public float playTime = 30f;
    public float wallSize = 8f;
    public float wallRotationSpeed = 45f;

    [HideInInspector] public Transform wallsParent;
    [HideInInspector] public BallController ball;
    [HideInInspector] public List<Target> targets = new List<Target>();

    public enum GameState { Playing, Clear, TimeUp }
    public GameState State { get; private set; }
    public int Score { get; private set; }
    public float ElapsedTime { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitOnLoad()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
        }
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        SetupField();
        SpawnBall();
        SpawnTargets();
        State = GameState.Playing;
    }

    void Update()
    {
        if (State != GameState.Playing) return;

        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= playTime)
        {
            EndGame(GameState.TimeUp);
        }

        float axis = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(axis) > 0.01f)
        {
            RotateWalls(-axis * wallRotationSpeed * Time.deltaTime);
        }

        if (targets.Count == 0)
        {
            EndGame(GameState.Clear);
        }
    }

    public void RotateWalls(float angle)
    {
        if (wallsParent != null)
            wallsParent.Rotate(Vector3.forward, angle);
    }

    public void EndGame(GameState result)
    {
        State = result;
        Debug.Log($"Game ended: {result} score {Score}");
    }

    void SetupField()
    {
        wallsParent = new GameObject("Walls").transform;
        float half = wallSize / 2f;
        CreateWall(new Vector2(0, half), new Vector2(wallSize, 0.5f));
        CreateWall(new Vector2(0, -half), new Vector2(wallSize, 0.5f));
        CreateWall(new Vector2(-half, 0), new Vector2(0.5f, wallSize));
        CreateWall(new Vector2(half, 0), new Vector2(0.5f, wallSize));
        wallsParent.rotation = Quaternion.Euler(0, 0, 45f);
    }

    void CreateWall(Vector2 pos, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.parent = wallsParent;
        wall.transform.position = pos;
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        var rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.sharedMaterial = new PhysicsMaterial2D { bounciness = 1f, friction = 0f };
    }

    void SpawnBall()
    {
        GameObject go = new GameObject("Ball");
        ball = go.AddComponent<BallController>();
        go.AddComponent<CircleCollider2D>();
        go.transform.position = Vector3.zero;
    }

    void SpawnTargets()
    {
        for (int i = 0; i < targetCount; i++)
        {
            GameObject go = new GameObject($"Target{i}");
            go.transform.position = Random.insideUnitCircle * (wallSize * 0.3f);
            var t = go.AddComponent<Target>();
            go.AddComponent<CircleCollider2D>().isTrigger = true;
            t.manager = this;
            targets.Add(t);
        }
    }

    public void TargetHit(Target t)
    {
        if (!targets.Contains(t)) return;
        targets.Remove(t);
        if (Application.isPlaying)
            Destroy(t.gameObject);
        else
            DestroyImmediate(t.gameObject);
        Score++;
    }
}
