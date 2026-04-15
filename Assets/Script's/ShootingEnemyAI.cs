using UnityEngine;

public class ShootingEnemyAI : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

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

    private float distance;
    private float damageTimer = 0f;
    private float shootTimer = 0f;

    void Update()
    {
        if (player == null) return;

        distance = Vector2.Distance(transform.position, player.transform.position);

        // Always recalculate direction every frame so rotation and shooting stay accurate
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < enemyMaxFollow)
        {
            // Always face the player while in range
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Only walk closer if outside the stop-chase range
            if (distance > stopChaseRange)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.transform.position,
                    enemySpeed * Time.deltaTime
                );
            }
        }

        // Shooting cooldown
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        // Shoot when in range and cooldown is ready
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
                if (anim != null) anim.SetTrigger("Hurt");
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
            if (ep != null) ep.expAmount = expDropAmount;
        }
        Destroy(gameObject);
    }
}