using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public float range = 20f;

    private float timer;

    void Update()
    {
        if (projectilePrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            timer = 0f;
            ShootNearestEnemy();
        }
    }

void ShootNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length == 0) return;

        Enemy nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Enemy e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < nearestDist && dist <= range)
            {
                nearestDist = dist;
                nearest = e;
            }
        }

        if (nearest == null) return;

        Vector3 dir = (nearest.transform.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        GameObject bullet = Instantiate(projectilePrefab, transform.position + dir * 0.6f, Quaternion.identity);
        bullet.SetActive(true);
        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }
}
