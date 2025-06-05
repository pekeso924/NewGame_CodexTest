using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int targetCount = 20;
    public float timeLimit = 30f;
    public GameObject arenaRoot;
    public Text scoreText;
    public Text timeText;

    int score;
    float timer;
    List<GameObject> targets = new List<GameObject>();
    GameObject ball;

    void Start()
    {
        CreateArena();
        SpawnBall();
        SpawnTargets();
        UpdateUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeLimit)
        {
            enabled = false;
        }
        UpdateUI();
    }

    void CreateArena()
    {
        arenaRoot = new GameObject("ArenaRoot");
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.SetParent(arenaRoot.transform);
        floor.transform.localScale = Vector3.one * 2f;
        floor.transform.rotation = Quaternion.Euler(0f, 0f, 45f);

        float size = 10f;
        for (int i = 0; i < 4; i++)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.SetParent(arenaRoot.transform);
            wall.transform.localScale = new Vector3(size * 2f, 1f, 1f);
            wall.transform.position = Quaternion.Euler(0f, i * 90f, 0f) * new Vector3(size, 0.5f, 0f);
        }
        arenaRoot.AddComponent<ArenaController>();
    }

    void SpawnBall()
    {
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var rb = ball.AddComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void SpawnTargets()
    {
        for (int i = 0; i < targetCount; i++)
        {
            var t = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            t.transform.position = new Vector3(Random.Range(-4f,4f),0.5f,Random.Range(-4f,4f));
            var col = t.GetComponent<SphereCollider>();
            col.isTrigger = true;
            t.AddComponent<Target>();
            targets.Add(t);
        }
    }

    public void AddScore()
    {
        score++;
        UpdateUI();
        if (score >= targetCount)
        {
            enabled = false;
        }
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = score.ToString();
        if (timeText) timeText.text = timer.ToString("F1");
    }
}
