using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("EXP Drop")]
    public GameObject expPickupPrefab;  
    public int expDropAmount = 2;       

    [Header("Death Flash (optional)")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private float flashTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f && spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
           
            BulletScript bullet = other.GetComponent<BulletScript>();
            int damage = (bullet != null) ? bullet.damage : 1;

            TakeDamage(damage);
            Destroy(other.gameObject); 
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            flashTimer = flashDuration;
        }

        if (currentHealth <= 0)
            Die();
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

        Destroy(gameObject);
    }
}
