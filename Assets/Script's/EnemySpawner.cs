using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private WaveTextUI waveTextUI;

    [Header("References")]
    public GameObject enemyType1Prefab;
    public GameObject enemyType1Wave3Prefab;
    public GameObject enemyType1Wave6Prefab;
    public GameObject enemyType2Prefab;
    public GameObject player;

    [Header("Spawning")]
    public float spawnRadius = 10f;
    public float spawnInterval = 1f;

    [Header("Wave Settings")]
    public int startingEnemies = 15;
    public int enemiesIncreasePerWave = 5;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float enemyType1Chance = 0.75f;
    [Range(0f, 1f)] public float enemyType2Chance = 0.25f;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;

    private bool waitingForNextWave = false;
    private float waveTimer = 0f;
    private float timer = 0f;

    private int currentWave = 1;
    private int enemiesToSpawnThisWave;
    private int enemiesSpawnedThisWave;
    private int enemiesAlive;

    private bool waveInProgress = false;

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        if (player == null) return;

        if (waitingForNextWave)
        {
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0f)
            {
                currentWave++;
                StartWave();
                waitingForNextWave = false;
            }

            return;
        }

        if (!waveInProgress) return;

        timer += Time.deltaTime;

        if (enemiesSpawnedThisWave < enemiesToSpawnThisWave && timer >= spawnInterval)
        {
            timer = 0f;
            SpawnSingleEnemy();
        }

        if (enemiesSpawnedThisWave >= enemiesToSpawnThisWave && enemiesAlive <= 0)
        {
            waveInProgress = false;
            waitingForNextWave = true;
            waveTimer = timeBetweenWaves;
        }
    }

    void StartWave()
    {
        enemiesToSpawnThisWave = startingEnemies + (currentWave - 1) * enemiesIncreasePerWave;
        enemiesSpawnedThisWave = 0;
        enemiesAlive = 0;
        timer = 0f;
        waveInProgress = true;

        if (waveTextUI != null)
            waveTextUI.ShowWave(currentWave);
    }

    void SpawnSingleEnemy()
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
            normalAI.spawner = this;
        }

        ShootingEnemyAI shootingAI = enemy.GetComponent<ShootingEnemyAI>();
        if (shootingAI != null)
        {
            shootingAI.player = player;
            shootingAI.spawner = this;
        }

        enemiesSpawnedThisWave++;
        enemiesAlive++;
    }

    GameObject GetRandomEnemyPrefab()
    {
        float totalChance = enemyType1Chance + enemyType2Chance;
        float randomValue = Random.Range(0f, totalChance);

        if (randomValue < enemyType1Chance)
            return GetCurrentEnemyType1Prefab();
        else
            return enemyType2Prefab;
    }

    GameObject GetCurrentEnemyType1Prefab()
    {
        if (currentWave > 5 && enemyType1Wave6Prefab != null)
            return enemyType1Wave6Prefab;

        if (currentWave > 2 && enemyType1Wave3Prefab != null)
            return enemyType1Wave3Prefab;

        return enemyType1Prefab;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}