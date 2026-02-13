using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class VampireSurvivorsGame : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float spawnInterval = 1.5f;
    public float spawnDistance = 12f;
    public float enemySpeed = 2f;
    public int enemyHealth = 3;
    public float fireRate = 0.5f;
    public float bulletSpeed = 12f;
    public float shootRange = 25f;

    private Rigidbody playerRb;
    private Vector3 moveInput;
    private float nextSpawnTime;
    private float nextShootTime;

    private List<GameObject> enemies = new List<GameObject>();
    private List<int> enemyHP = new List<int>();
    private List<GameObject> bullets = new List<GameObject>();
    private List<Vector3> bulletDir = new List<Vector3>();
    private List<float> bulletLife = new List<float>();

    private Material enemyMat;
    private Material bulletMat;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        var renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            var baseMat = renderer.material;
            enemyMat = new Material(baseMat);
            enemyMat.color = Color.red;
            bulletMat = new Material(baseMat);
            bulletMat.color = Color.yellow;
        }

        var cam = Camera.main;
        if (cam != null)
        {
            var follow = cam.GetComponent<CameraFollow>();
            if (follow != null) follow.target = transform;
        }

        nextSpawnTime = 0f;
        nextShootTime = 0f;
    }

    void Update()
    {
        if (enemyMat == null || bulletMat == null) return;

        float dt = Time.deltaTime;

        // Input
        Vector2 inp = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed) inp.y += 1f;
            if (kb.sKey.isPressed) inp.y -= 1f;
            if (kb.aKey.isPressed) inp.x -= 1f;
            if (kb.dKey.isPressed) inp.x += 1f;
        }
        moveInput = new Vector3(inp.x, 0f, inp.y).normalized;

        // Spawn enemies
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnDistance;
            pos.y = 0.5f;
            GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube);
            e.name = "Enemy";
            e.transform.position = pos;
            e.transform.localScale = Vector3.one * 0.8f;
            var r = e.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = enemyMat;
            enemies.Add(e);
            enemyHP.Add(enemyHealth);
        }

        // Move enemies toward player
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null) { enemies.RemoveAt(i); enemyHP.RemoveAt(i); continue; }
            Vector3 d = (transform.position - enemies[i].transform.position).normalized;
            enemies[i].transform.position += d * enemySpeed * dt;
        }

        // Shoot at nearest enemy
        if (Time.time >= nextShootTime && enemies.Count > 0)
        {
            nextShootTime = Time.time + fireRate;
            GameObject nearest = null;
            float nd = float.MaxValue;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                float dd = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (dd < nd && dd <= shootRange) { nd = dd; nearest = enemies[i]; }
            }
            if (nearest != null)
            {
                Vector3 dir = nearest.transform.position - transform.position;
                dir.y = 0f;
                dir.Normalize();
                GameObject b = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                b.name = "Bullet";
                b.transform.position = transform.position + dir * 0.6f + Vector3.up * 0.5f;
                b.transform.localScale = Vector3.one * 0.25f;
                var br = b.GetComponent<Renderer>();
                if (br != null) br.sharedMaterial = bulletMat;
                var col = b.GetComponent<Collider>();
                if (col != null) Destroy(col);
                bullets.Add(b);
                bulletDir.Add(dir);
                bulletLife.Add(3f);
            }
        }

        // Move bullets and check hits
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (i >= bullets.Count) break;
            if (bullets[i] == null) { bullets.RemoveAt(i); bulletDir.RemoveAt(i); bulletLife.RemoveAt(i); continue; }
            bulletLife[i] -= dt;
            if (bulletLife[i] <= 0f)
            {
                Destroy(bullets[i]);
                bullets.RemoveAt(i); bulletDir.RemoveAt(i); bulletLife.RemoveAt(i);
                continue;
            }
            bullets[i].transform.position += bulletDir[i] * bulletSpeed * dt;

            bool hit = false;
            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                if (enemies[j] == null) continue;
                if (Vector3.Distance(bullets[i].transform.position, enemies[j].transform.position) < 0.7f)
                {
                    enemyHP[j]--;
                    if (enemyHP[j] <= 0)
                    {
                        Destroy(enemies[j]);
                        enemies.RemoveAt(j);
                        enemyHP.RemoveAt(j);
                    }
                    if (i < bullets.Count && bullets[i] != null)
                    {
                        Destroy(bullets[i]);
                        bullets.RemoveAt(i);
                        bulletDir.RemoveAt(i);
                        bulletLife.RemoveAt(i);
                    }
                    hit = true;
                    break;
                }
            }
            if (hit) continue;
        }
    }

    void FixedUpdate()
    {
        if (playerRb != null)
            playerRb.MovePosition(playerRb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
