using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator anim;
    private Rigidbody2D rb; // ✅ Changed to 2D

    private Vector2 movement; // ✅ Changed to Vector2

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // ✅ Changed to 2D
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical"); // ✅ Y axis instead of Z for 2D

        movement = new Vector2(moveX, moveY); // ✅ Vector2

        anim.SetFloat("Speed", movement.magnitude);

        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("Attack");

        if (Input.GetKeyDown(KeyCode.R))
            anim.SetTrigger("Reload");

        if (Input.GetKeyDown(KeyCode.H))
            anim.SetTrigger("Hurt");
    }

    void FixedUpdate()
    {
        // ✅ MovePosition with Vector2
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // ✅ 2D rotation uses a float angle, not a Quaternion
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, 10f * Time.fixedDeltaTime);
        }
    }
}