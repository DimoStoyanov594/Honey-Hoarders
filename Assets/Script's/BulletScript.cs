using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float force = 10f;
    public float lifetime = 3f;

    private Vector2 direction;
    private bool directionSet = false;

    private int damage = 1;

   public void SetDamage(int dmg)
   {
        damage = dmg;
    }

    public int GetDamage()
    {
        return damage;
    }

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
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}