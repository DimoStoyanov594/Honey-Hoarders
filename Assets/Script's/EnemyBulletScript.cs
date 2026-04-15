using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 5f;
    public int damage = 1;
    public float lifetime = 4f;       // Auto-destroys after this many seconds

    private Vector2 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Called by ShootingEnemyAI right after instantiation to set the travel direction.
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Rotate the sprite to face the direction it's travelling
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(180f, 180f, angle);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthManager hm = collision.GetComponent<HealthManager>();
            if (hm != null)
            {
                hm.TakeDamage(damage);
                Animator anim = collision.GetComponent<Animator>();
                if (anim != null) anim.SetTrigger("Hurt");
            }
            Destroy(gameObject);
        }

        // Destroy on hitting anything tagged as ground / wall / obstacle
        // Add more tags below if needed
        if (collision.CompareTag("Bullet") || collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}