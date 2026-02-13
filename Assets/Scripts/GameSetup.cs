using UnityEngine;

public class GameSetup : MonoBehaviour
{
void Awake()
    {
        // Create enemy prefab template
        GameObject enemyTemplate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyTemplate.name = "EnemyTemplate";
        enemyTemplate.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        enemyTemplate.AddComponent<Enemy>();
        var enemyRenderer = enemyTemplate.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
        }
        enemyTemplate.SetActive(false);
        DontDestroyOnLoad(enemyTemplate);

        // Create projectile prefab template
        GameObject projTemplate = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projTemplate.name = "ProjectileTemplate";
        projTemplate.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        projTemplate.AddComponent<Projectile>();
        var projRb = projTemplate.AddComponent<Rigidbody>();
        projRb.isKinematic = true;
        projRb.useGravity = false;
        var projCol = projTemplate.GetComponent<SphereCollider>();
        if (projCol != null) projCol.isTrigger = true;
        var projRenderer = projTemplate.GetComponent<Renderer>();
        if (projRenderer != null)
        {
            projRenderer.material.color = Color.yellow;
        }
        projTemplate.SetActive(false);
        DontDestroyOnLoad(projTemplate);

        // Wire up AutoShooter
        var shooter = FindFirstObjectByType<AutoShooter>();
        Debug.Log($"[GameSetup] AutoShooter found: {shooter != null}");
        if (shooter != null)
        {
            shooter.projectilePrefab = projTemplate;
        }

        // Wire up EnemySpawner
        var spawner = FindFirstObjectByType<EnemySpawner>();
        Debug.Log($"[GameSetup] EnemySpawner found: {spawner != null}");
        if (spawner != null)
        {
            spawner.enemyPrefab = enemyTemplate;
            Debug.Log($"[GameSetup] enemyPrefab assigned: {spawner.enemyPrefab != null}");
        }

        // Wire up CameraFollow target
        var cam = FindFirstObjectByType<CameraFollow>();
        var player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"[GameSetup] Camera: {cam != null}, Player: {player != null}");
        if (cam != null && cam.target == null && player != null)
        {
            cam.target = player.transform;
        }
    }
}
