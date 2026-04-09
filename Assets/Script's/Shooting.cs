using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;

    public Transform bulletTransform;
    public Transform rotatePoint;
    public float timeBetweenFiring = 0.2f;

    private bool canFire = true;
    private float timer;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);

        mousePos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mousePos - rotatePoint.position;

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rotatePoint.rotation = Quaternion.Euler(0, 0, rotZ - 185f);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            BulletPool.Instance.Get(bulletTransform.position, bulletTransform.rotation);
        }
    }
}