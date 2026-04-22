using UnityEngine;

public class ShootingEnemyAI : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public EnemySpawner spawner;

    [Header("Movement")]
    public float enemySpeed = 2f;
    public float enemyMaxFollow = 20f;

    [Header("Health & Damage")]
    public float enemyHealth = 3f;
    public float bulletDamage = 1f;

    [Header("Player Contact Damage")]
    public int damageToPlayer = 1;
    public float damageCooldown = 1f;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public float shootRange = 10f;
    public float stopChaseRange = 6f;
    public float fireRate = 2f;
    public float bulletSpeed = 5f;
    public int bulletDamageToPlayer = 1;

    [Header("EXP Drop")]
    public GameObject expPickupPrefab;
    public int expDropAmount = 3;

    [Header("Enemy Overlap")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float overlapCheckRadius = 1f;

    private float distance;
    private float damageTimer = 0f;
    private float shootTimer = 0f;

    private Collider2D ownCollider;
    private readonly Collider2D[] overlapResults = new Collider2D[16];

    void Start()
    {
        ownCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (player == null) return;

        distance = Vector2.Distance(transform.position, player.transform.position);

        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < enemyMaxFollow)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (distance > stopChaseRange)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.transform.position,
                    enemySpeed * Time.deltaTime
                );
            }
        }

        ResolveEnemyOverlap();

        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        if (distance < shootRange && shootTimer <= 0f)
        {
            Shoot(direction);
            shootTimer = fireRate;
        }

        if (enemyHealth <= 0)
            Die();

        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;
    }

    private void ResolveEnemyOverlap()
    {
        if (ownCollider == null)
            return;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = enemyLayer;
        filter.useTriggers = true;

        int count = Physics2D.OverlapCircle(transform.position, overlapCheckRadius, filter, overlapResults);

        for (int i = 0; i < count; i++)
        {
            Collider2D other = overlapResults[i];

            if (other == null || other == ownCollider)
                continue;

            ColliderDistance2D distanceInfo = ownCollider.Distance(other);

            if (distanceInfo.isOverlapped)
            {
                Vector2 pushDir = -distanceInfo.normal;
                float pushAmount = -distanceInfo.distance;

                transform.position += (Vector3)(pushDir * pushAmount * 0.5f);
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if (enemyBulletPrefab == null) return;

        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);

        EnemyBulletScript ebs = bullet.GetComponent<EnemyBulletScript>();
        if (ebs != null)
        {
            ebs.SetDirection(direction);
            ebs.speed = bulletSpeed;
            ebs.damage = bulletDamageToPlayer;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            BulletScript bullet = collision.GetComponent<BulletScript>();
            enemyHealth -= (bullet != null) ? bullet.GetDamage() : bulletDamage;
            return;
        }

        if (collision.CompareTag("CompanionBullet"))
        {
            CompanionBullet companionBullet = collision.GetComponent<CompanionBullet>();
            if (companionBullet != null)
            {
                enemyHealth -= companionBullet.GetDamage();
                companionBullet.DeactivateBullet();
            }
            return;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (damageTimer > 0f) return;

        if (other.CompareTag("Player"))
        {
            HealthManager hm = other.GetComponent<HealthManager>();
            if (hm != null)
            {
                hm.TakeDamage(damageToPlayer);

                Animator anim = other.GetComponent<Animator>();
                if (anim != null)
                    anim.SetTrigger("Hurt");

                damageTimer = damageCooldown;
            }
        }
    }

    void Die()
    {
        if (expPickupPrefab != null)
        {
            GameObject pickup = Instantiate(expPickupPrefab, transform.position, Quaternion.identity);
            ExpPickup ep = pickup.GetComponent<ExpPickup>();
            if (ep != null)
                ep.expAmount = expDropAmount;
        }

        if (spawner != null)
            spawner.OnEnemyKilled();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, overlapCheckRadius);
    }
}