using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private Transform player;

    [Header("Shooting")]
    [SerializeField] private float timeBetweenFiring = 0.2f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float rotationOffset = -185f;

    private Camera mainCam;
    private float timer;
    private bool pauseAimAndShooting = false;

    private void OnEnable()
    {
        PauseManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        PauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    private void Start()
    {
        mainCam = Camera.main;

        if (player == null)
            player = transform.root;

        if (PauseManager.Instance != null)
            pauseAimAndShooting = PauseManager.Instance.IsPaused;
    }

    private void Update()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (pauseAimAndShooting)
            return;

        timer += Time.deltaTime;

        UpdateAim();
        HandleShooting();
    }

    private void HandlePauseStateChanged(bool paused)
    {
        pauseAimAndShooting = paused;
    }

    private void UpdateAim()
    {
        if (mainCam == null)
            return;

        Transform rotationTarget = aimPivot != null ? aimPivot : transform;

        if (aimPivot != null && player != null)
            aimPivot.position = player.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = mouseWorldPos - rotationTarget.position;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotationTarget.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && timer >= timeBetweenFiring)
        {
            timer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
            return;

        if (firePoint == null)
            return;

        if (mainCam == null)
            return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 bulletDirection = (mouseWorldPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletScript bs = bullet.GetComponent<BulletScript>();
        if (bs != null)
        {
            bs.SetDamage(bulletDamage);
            bs.SetDirection(bulletDirection);
        }
    }

    public void SetBulletDamage(int newDamage)
    {
        bulletDamage = newDamage;
    }

    public int GetBulletDamage()
    {
        return bulletDamage;
    }

    public float GetTimeBetweenFiring()
    {
        return timeBetweenFiring;
    }

    public void SetTimeBetweenFiring(float newTimeBetweenFiring)
    {
        timeBetweenFiring = Mathf.Max(0.05f, newTimeBetweenFiring);
    }

    public void SetFirePoint(Transform newFirePoint)
    {
        firePoint = newFirePoint;
    }

    public void SetAimPivot(Transform newAimPivot)
    {
        aimPivot = newAimPivot;
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}