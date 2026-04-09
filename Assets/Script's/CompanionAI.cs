using UnityEngine;
using System.Collections.Generic;

public class CompanionAI : MonoBehaviour
{
    [Header("Following")]
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float lag = 0.12f;

    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int poolSize = 10;

    [Header("Optional")]
    [SerializeField] private bool requireLineOfSight = false;

    private Transform player;
    private Vector3 velocity = Vector3.zero;
    private float fireCooldown = 0f;
    private Queue<GameObject> projectilePool = new Queue<GameObject>();

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        BuildPool();
    }

    private void BuildPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(projectilePrefab);
            b.SetActive(false);
            projectilePool.Enqueue(b);
        }
    }

    private GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject b;

        if (projectilePool.Count > 0)
            b = projectilePool.Dequeue();
        else
            b = Instantiate(projectilePrefab);

        b.transform.position = position;
        b.transform.rotation = rotation;
        b.SetActive(true);
        return b;
    }

    public void ReturnToPool(GameObject b)
    {
        b.SetActive(false);
        projectilePool.Enqueue(b);
    }

    private void Update()
    {
        if (player == null) return;

        HandleFollowing();
        HandleCombat();
    }

    private void HandleFollowing()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > stopDistance)
        {
            Vector3 target = player.position;

            if (target.x > transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                target,
                ref velocity,
                lag,
                followSpeed
            );
        }
    }

    private void HandleCombat()
    {
        fireCooldown -= Time.deltaTime;

        Transform nearest = GetNearestEnemy();
        if (nearest == null) return;

        if (fireCooldown <= 0f)
        {
            ShootAt(nearest);
            fireCooldown = 1f / fireRate;
        }
    }

    private Transform GetNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange,
            enemyLayer
        );

        Transform nearest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == player)
                continue;

            if (requireLineOfSight)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;

                RaycastHit2D ray = Physics2D.Raycast(
                    transform.position,
                    direction,
                    detectionRange,
                    enemyLayer
                );

                if (ray.collider == null || ray.collider.transform != hit.transform)
                    continue;
            }

            float d = Vector3.Distance(transform.position, hit.transform.position);

            if (d < closestDist)
            {
                closestDist = d;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void ShootAt(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        if (target.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject proj = GetFromPool(
            transform.position,
            Quaternion.Euler(0, 0, angle)
        );

        CompanionBullet bullet = proj.GetComponent<CompanionBullet>();
        if (bullet != null)
            bullet.Init(this);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}