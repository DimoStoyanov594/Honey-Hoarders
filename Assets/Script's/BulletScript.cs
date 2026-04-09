using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Camera mainCam;
    private Rigidbody2D rb;

    public float force = 10f;
    public float lifetime = 3f;
    private float timer;

    private void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Reset lifetime timer each time bullet is reused
        timer = 0f;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);

        Vector3 mousePos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        Vector3 direction = mousePos - transform.position;

        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    private void Update()
    {
        // Return to pool after lifetime instead of Destroy
        timer += Time.deltaTime;
        if (timer >= lifetime)
            BulletPool.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Return to pool on hit instead of Destroy
        BulletPool.Instance.ReturnToPool(gameObject);
    }
}