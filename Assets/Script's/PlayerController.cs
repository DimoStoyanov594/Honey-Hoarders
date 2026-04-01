using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator anim;
    private Rigidbody2D rb;
    private HealthManager healthManager;

    private Vector2 movement;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movement = new Vector2(moveX, moveY);

        anim.SetFloat("Speed", movement.magnitude);

        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("Attack");

        if (Input.GetKeyDown(KeyCode.R))
            anim.SetTrigger("Reload");

        if (Input.GetKeyDown(KeyCode.H))
            anim.SetTrigger("Hurt");

        // Press P to test damage
        if (Input.GetKeyDown(KeyCode.P))
            TestDamage();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, 10f * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            healthManager.TakeDamage(1);
            anim.SetTrigger("Hurt");
        }
    }

    void TestDamage()
    {
        healthManager.TakeDamage(1);
        anim.SetTrigger("Hurt");
    }
}