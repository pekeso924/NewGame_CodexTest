using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float arenaSize = 5f;
    public float rotationSpeed = 90f;
    public float timeLimit = 30f;
    public int initialTargetCount = 5;

    public GameObject arena;
    public BallController ball;
    public List<GameObject> targets = new List<GameObject>();
    public int score;
    public float timeLeft;

    public void Initialize()
    {
        timeLeft = timeLimit;
        score = 0;
        if (arena != null) DestroyImmediate(arena);
        if (ball != null) DestroyImmediate(ball.gameObject);
        foreach (var t in targets) if (t) DestroyImmediate(t);
        targets.Clear();

        arena = CreateArena();
        ball = CreateBall();
        CreateTargets(initialTargetCount);
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        RotateArena(input, Time.deltaTime);
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            EndGame(false);
        }
        if (targets.Count == 0)
        {
            EndGame(true);
        }
    }

    public void RotateArena(float input, float deltaTime)
    {
        if (arena != null)
            arena.transform.Rotate(0f, 0f, -input * rotationSpeed * deltaTime);
    }

    GameObject CreateArena()
    {
        GameObject parent = new GameObject("Arena");
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall" + i;
            wall.transform.SetParent(parent.transform);
            wall.transform.localScale = new Vector3(arenaSize * 2f, 0.2f, 1f);
            float angle = i * 90f;
            wall.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            wall.transform.localPosition = Quaternion.Euler(0f, 0f, angle) * new Vector3(arenaSize, 0f, 0f);
            var col = wall.GetComponent<BoxCollider>();
            var mat = new PhysicMaterial();
            mat.bounciness = 1f;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            mat.bounceCombine = PhysicMaterialCombine.Maximum;
            col.material = mat;
            var rb = wall.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        return parent;
    }

    BallController CreateBall()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "Ball";
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one * 0.5f;
        var rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        var col = obj.GetComponent<SphereCollider>();
        var mat = new PhysicMaterial();
        mat.bounciness = 1f;
        mat.frictionCombine = PhysicMaterialCombine.Minimum;
        mat.bounceCombine = PhysicMaterialCombine.Maximum;
        col.material = mat;
        return obj.AddComponent<BallController>();
    }

    void CreateTargets(int count)
    {
        float radius = arenaSize * 0.6f;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Random.insideUnitCircle.normalized * radius;
            CreateTarget(pos);
        }
    }

    public GameObject CreateTarget(Vector3 position)
    {
        GameObject t = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        t.name = "Target" + targets.Count;
        t.transform.position = position;
        t.transform.localScale = Vector3.one * 0.5f;
        var col = t.GetComponent<SphereCollider>();
        col.isTrigger = true;
        t.GetComponent<Renderer>().sharedMaterial.color = new Color(1f, 0.5f, 0f);
        var tc = t.AddComponent<TargetController>();
        tc.manager = this;
        targets.Add(t);
        return t;
    }

    public void TargetHit(GameObject target)
    {
        if (targets.Remove(target))
        {
            if (Application.isPlaying)
                Destroy(target);
            else
                DestroyImmediate(target);
            score++;
        }
    }

    void EndGame(bool cleared)
    {
        Debug.Log(cleared ? "CLEARED" : "TIMEUP");
        enabled = false;
    }
}
