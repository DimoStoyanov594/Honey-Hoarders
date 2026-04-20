using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Animator anim;

    [Header("Damage")]
    [SerializeField] private float invulnerabilityTime = 0.75f;

    private Rigidbody2D rb;
    private HealthManager healthManager;
    private PlayerDamageIndicator damageIndicator;

    private Vector2 movement;
    private bool isInvulnerable = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthManager = GetComponent<HealthManager>();
        damageIndicator = GetComponent<PlayerDamageIndicator>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movement = new Vector2(moveX, moveY);

        if (anim != null)
        {
            anim.SetFloat("Speed", movement.magnitude);

            if (Input.GetMouseButtonDown(0))
                anim.SetTrigger("Attack");

            if (Input.GetKeyDown(KeyCode.R))
                anim.SetTrigger("Reload");

            if (Input.GetKeyDown(KeyCode.H))
                anim.SetTrigger("Hurt");
        }

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
            TakePlayerDamage(1);
        }
    }

    public void TakePlayerDamage(int amount)
    {
        if (isInvulnerable)
            return;

        healthManager.TakeDamage(amount);

        if (anim != null)
            anim.SetTrigger("Hurt");

        if (damageIndicator != null)
            damageIndicator.ShowDamageBlink();

        StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    void TestDamage()
    {
        healthManager.TakeDamage(1);

        if (anim != null)
            anim.SetTrigger("Hurt");
    }
}