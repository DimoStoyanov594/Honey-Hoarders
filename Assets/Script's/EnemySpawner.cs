using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyType1Prefab;
    public GameObject enemyType2Prefab;
    public GameObject player;

    [Header("Spawning")]
    public float spawnRadius = 10f;
    public float spawnInterval = 2f;
    public int enemiesPerWave = 1;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float enemyType1Chance = 0.75f;
    [Range(0f, 1f)] public float enemyType2Chance = 0.25f;

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

            GameObject chosenPrefab = GetRandomEnemyPrefab();
            GameObject enemy = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

            EnemyAI normalAI = enemy.GetComponent<EnemyAI>();
            if (normalAI != null)
            {
                normalAI.player = player;
            }

            ShootingEnemyAI shootingAI = enemy.GetComponent<ShootingEnemyAI>();
            if (shootingAI != null)
            {
                shootingAI.player = player;
            }
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        float totalChance = enemyType1Chance + enemyType2Chance;
        float randomValue = Random.Range(0f, totalChance);

        if (randomValue < enemyType1Chance)
            return enemyType1Prefab;
        else
            return enemyType2Prefab;
    }
}