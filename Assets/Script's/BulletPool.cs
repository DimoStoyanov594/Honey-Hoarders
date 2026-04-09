using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Pre-spawn all bullets and deactivate them
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(bulletPrefab);
            b.SetActive(false);
            pool.Enqueue(b);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject b;

        if (pool.Count > 0)
            b = pool.Dequeue();
        else
        {
            // Pool ran out — create a new one as fallback
            b = Instantiate(bulletPrefab);
        }

        b.transform.position = position;
        b.transform.rotation = rotation;
        b.SetActive(true);
        return b;
    }

    public void ReturnToPool(GameObject b)
    {
        b.SetActive(false);
        pool.Enqueue(b);
    }
}