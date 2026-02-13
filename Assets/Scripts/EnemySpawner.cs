using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.5f;
    public float spawnDistance = 15f;

    private Transform player;
    private float timer;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

void Update()
    {
        if (player == null || enemyPrefab == null)
        {
            Debug.Log($"[Spawner] player={player != null} prefab={enemyPrefab != null}");
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

void SpawnEnemy()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnDistance;
        Vector3 spawnPos = player.position + offset;
        spawnPos.y = 0.5f;
        Debug.Log($"[Spawner] Spawning enemy at {spawnPos}");
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.SetActive(true);
        Debug.Log($"[Spawner] Enemy active: {enemy.activeSelf}, has Enemy: {enemy.GetComponent<Enemy>() != null}");
    }
}
