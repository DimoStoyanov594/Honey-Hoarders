using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public GameObject player;

    [Header("Spawning")]
    public float spawnRadius = 10f;     
    public float spawnInterval = 2f;    
    public int enemiesPerWave = 1;   
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 spawnPos = new Vector2(
                player.transform.position.x + Mathf.Cos(angle) * spawnRadius,
                player.transform.position.y + Mathf.Sin(angle) * spawnRadius
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
                ai.player = player;
        }
    }
}
