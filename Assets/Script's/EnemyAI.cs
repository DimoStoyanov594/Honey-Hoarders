using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public GameObject player; 
    public EnemySpawner spawner;

    [Header("Movement")]
    public float enemySpeed = 3f;
    public float enemyMaxFollow = 20f;

    [Header("Health & Damage")]
    public float enemyHealth = 3f;
    public float bulletDamage = 1f;

    [Header("Player Damage")]
    public int damageToPlayer = 1;
    public float damageCooldown = 1f;

    [Header("EXP Drop")]
    public GameObject expPickupPrefab;
    public int expDropAmount = 2;

    private float distance;
    private float damageTimer = 0f;

    void Update()
    {
        if (player == null) return;

        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < enemyMaxFollow)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                enemySpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }

        if (enemyHealth <= 0)
            Die();

        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;
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
}