using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;

    [Tooltip("Drag the bullet PREFAB from your Project panel here — NOT from the Hierarchy")]
    public GameObject bulletPrefab;
    public Transform firePoint;        
    public float timeBetweenFiring = 0.2f;

    private float timer = 0f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mousePos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mousePos - transform.position;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 185f);


        if (Input.GetMouseButton(0) && timer >= timeBetweenFiring)
        {
            timer = 0f;
            Shoot(mousePos);
        }
    }

    void Shoot(Vector3 mousePos)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Shooting: bulletPrefab is not assigned! Drag the bullet prefab from your Project panel.");
            return;
        }

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

  
        BulletScript bs = b.GetComponent<BulletScript>();
        if (bs != null)
            bs.SetDirection((mousePos - firePoint.position).normalized);
    }
}
