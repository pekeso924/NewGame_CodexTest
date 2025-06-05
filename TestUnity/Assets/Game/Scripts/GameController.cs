using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public BallController ballPrefab;
    public Target targetPrefab;
    public int targetCount = 20;
    public float timeLimit = 30f;

    public int Score { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool GameOver { get; private set; }
    public int ActiveTargetCount => _targets.Count;

    private BallController _ballInstance;
    private readonly List<Target> _targets = new List<Target>();

    public void StartGame()
    {
        ClearExisting();

        _ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        TimeRemaining = timeLimit;
        Score = 0;
        GameOver = false;

        for (int i = 0; i < targetCount; i++)
        {
            Vector3 pos = new Vector3(i % 5 - 2, 0.5f, i / 5 - 2);
            Target t = Instantiate(targetPrefab, pos, Quaternion.identity);
            _targets.Add(t);
        }
    }

    public void Tick(float dt)
    {
        if (GameOver)
            return;

        TimeRemaining -= dt;
        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            GameOver = true;
            return;
        }

        _targets.RemoveAll(t => t == null);
        if (_targets.Count == 0)
        {
            GameOver = true;
        }
    }

    public void TargetDestroyed(Target t)
    {
        Score += 1;
        _targets.Remove(t);
    }

    private void ClearExisting()
    {
        if (_ballInstance)
            DestroyImmediate(_ballInstance.gameObject);
        foreach (var t in _targets)
        {
            if (t)
                DestroyImmediate(t.gameObject);
        }
        _targets.Clear();
    }

    void Update()
    {
        Tick(Time.deltaTime);
    }
}
