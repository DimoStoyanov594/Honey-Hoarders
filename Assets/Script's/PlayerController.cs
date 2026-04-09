using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator anim;
    private Rigidbody2D rb;
    private HealthManager healthManager;
    private SpriteRenderer sr;

    private Vector2 movement;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthManager = GetComponent<HealthManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movement = new Vector2(moveX, moveY);

        anim.SetFloat("Speed", movement.magnitude);

        // Flip sprite based on horizontal movement
        if (moveX > 0)
            sr.flipX = true;  // moving right, flip
        else if (moveX < 0)
            sr.flipX = false; // moving left, default

        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("Attack");

        if (Input.GetKeyDown(KeyCode.R))
            anim.SetTrigger("Reload");

        if (Input.GetKeyDown(KeyCode.H))
            anim.SetTrigger("Hurt");

        if (Input.GetKeyDown(KeyCode.P))
            TestDamage();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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