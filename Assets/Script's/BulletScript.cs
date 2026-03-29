using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float force = 10f;
    public int damage = 1;
    public float lifetime = 3f;

    private Vector2 direction;
    private bool directionSet = false;

    
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        directionSet = true;

        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!directionSet) return;
        transform.Translate(direction * force * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;
        if (!other.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
