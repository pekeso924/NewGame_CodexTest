using System.Collections.Generic;
using UnityEngine;

public class SimpleText : MonoBehaviour
{
    public string text;
}

public class GameManager : MonoBehaviour
{
    public float timeLimit = 30f;
    public int targetCount = 20;
    public float arenaSize = 5f;
    public float rotationSpeed = 60f;

    private float _time;
    private int _score;
    public SimpleText scoreText;
    public SimpleText timeText;
    public Transform arenaRoot;
    public GameObject ballPrefab;
    public GameObject targetPrefab;
    private readonly List<GameObject> _targets = new List<GameObject>();
    public bool GameOver { get; private set; }

    public void Start()
    {
        if (!arenaRoot) CreateArena();
        if (!ballPrefab) ballPrefab = CreateBallPrefab();
        if (!targetPrefab) targetPrefab = CreateTargetPrefab();

        SpawnBall();
        SpawnTargets();
        UpdateUI();
    }

    void Update()
    {
        Tick(Time.deltaTime, Input.GetAxis("Horizontal"));
    }

    public void Tick(float dt, float input)
    {
        if (GameOver) return;

        _time += dt;
        RotateArena(input, dt);
        if (_time >= timeLimit)
            EndGame();
        UpdateUI();
    }

    public void RegisterHit(Target t)
    {
        if (GameOver) return;
        _score++;
        _targets.Remove(t.gameObject);
        if (_targets.Count == 0)
            EndGame();
        UpdateUI();
    }

    void EndGame()
    {
        GameOver = true;
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"SCORE: {_score}";
        if (timeText) timeText.text = $"TIME: {_time:F2}";
    }

    void RotateArena(float input, float dt)
    {
        if (arenaRoot)
            arenaRoot.Rotate(0f, 0f, input * rotationSpeed * dt);
    }

    void CreateArena()
    {
        var root = new GameObject("ArenaRoot");
        arenaRoot = root.transform;

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.SetParent(arenaRoot);
        floor.transform.localScale = Vector3.one;
        floor.transform.rotation = Quaternion.Euler(0f, 45f, 0f);

        float half = arenaSize;
        CreateWall(new Vector3(-half, 0.5f, 0f));
        CreateWall(new Vector3(half, 0.5f, 0f));
        CreateWall(new Vector3(0f, 0.5f, -half));
        CreateWall(new Vector3(0f, 0.5f, half));
    }

    void CreateWall(Vector3 pos)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(arenaRoot);
        wall.transform.localScale = new Vector3(arenaSize * 2f, 1f, 1f);
        wall.transform.position = pos;
    }

    GameObject CreateBallPrefab()
    {
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<Rigidbody>();
        ball.AddComponent<BallMarker>();
        return ball;
    }

    GameObject CreateTargetPrefab()
    {
        var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sc = target.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        target.AddComponent<Target>();
        return target;
    }

    void SpawnBall()
    {
        var obj = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        obj.name = "Ball";
    }

    void SpawnTargets()
    {
        var rng = new System.Random(0);
        for (int i = 0; i < targetCount; i++)
        {
            float x = (float)(rng.NextDouble() * arenaSize - arenaSize / 2);
            float z = (float)(rng.NextDouble() * arenaSize - arenaSize / 2);
            var target = Instantiate(targetPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
            target.SetActive(true);
            target.GetComponent<Target>().manager = this;
            _targets.Add(target);
        }
    }
}

public class BallMarker : MonoBehaviour {}
