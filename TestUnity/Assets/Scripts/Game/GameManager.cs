using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int targetCount = 5;
    public float timeLimit = 30f;
    public float arenaSize = 8f;
    public float wallThickness = 0.5f;
    public Ball ballPrefab; // not used since we create ball script directly

    public Ball BallInstance { get; private set; }
    public List<Target> Targets { get; private set; } = new List<Target>();
    public bool GameEnded { get; private set; }
    public int Score { get; private set; }
    public float TimeRemaining { get; private set; }

    void Start()
    {
        SetupScene();
    }

    public void SetupScene()
    {
        TimeRemaining = timeLimit;
        GameEnded = false;
        Score = 0;

        // Playfield parent rotated 45 degrees
        var playfield = new GameObject("Playfield");
        playfield.transform.rotation = Quaternion.Euler(0, 0, 45f);
        var rotator = playfield.AddComponent<WallRotator>();

        // Create 4 walls
        CreateWall(playfield.transform, new Vector2(-arenaSize / 2, 0), Vector2.one);
        CreateWall(playfield.transform, new Vector2(arenaSize / 2, 0), Vector2.one);
        CreateWall(playfield.transform, new Vector2(0, arenaSize / 2), new Vector2(1,1), 90f);
        CreateWall(playfield.transform, new Vector2(0, -arenaSize / 2), new Vector2(1,1), 90f);

        // Create ball
        var ballObj = new GameObject("Ball");
        ballObj.transform.position = Vector2.zero;
        var rb = ballObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = ballObj.AddComponent<CircleCollider2D>();
        BallInstance = ballObj.AddComponent<Ball>();
        BallInstance.Launch();

        // Create targets
        Targets.Clear();
        for (int i = 0; i < targetCount; i++)
        {
            float angle = (360f / targetCount) * i;
            Vector2 pos = Quaternion.Euler(0, 0, angle) * Vector2.up * (arenaSize / 3);
            var targObj = new GameObject($"Target{i}");
            targObj.transform.position = pos;
            var tcol = targObj.AddComponent<CircleCollider2D>();
            tcol.isTrigger = true;
            var target = targObj.AddComponent<Target>();
            target.Manager = this;
            Targets.Add(target);
        }
    }

    void CreateWall(Transform parent, Vector2 position, Vector2 scale, float rotation = 0f)
    {
        var wall = new GameObject("Wall");
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        wall.transform.localScale = new Vector3(wallThickness, arenaSize, 1);
        var col = wall.AddComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (GameEnded) return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f)
        {
            EndGame(false);
        }

        if (Targets.Count == 0)
        {
            EndGame(true);
        }
    }

    internal void TargetDestroyed(Target t)
    {
        if (Targets.Contains(t))
        {
            Targets.Remove(t);
            Score++;
        }
    }

    void EndGame(bool cleared)
    {
        GameEnded = true;
        Debug.Log($"Game Ended {(cleared ? "Clear" : "TimeUp")} Score={Score}");
    }
}
