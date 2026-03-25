using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;

    public float force = 10f;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // Get correct mouse world position
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);

        mousePos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        // Calculate direction
        Vector3 direction = mousePos - transform.position;

        // Apply velocity
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }
}