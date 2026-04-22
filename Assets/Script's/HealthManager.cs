using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("Starting Health")]
    public int startingHealth = 3;
    public int startingMaxHealth = 3;

    [Header("Limits")]
    public int absoluteMaxHealth = 6;

    [Header("Runtime Health")]
    public int health;
    public int maxHealth;

    [Header("UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public CanvasGroup heartContainer;

    [Header("Fade")]
    public float displayDuration = 2f;
    public float fadeDuration = 1f;

    [Header("Death")]
    [SerializeField] private GameOverManager gameOverManager;

    private float hideTimer = 0f;
    private bool isFading = false;
    private bool isDead = false;
    private List<Image> heartImages = new List<Image>();

    void Start()
    {
        heartImages = new List<Image>(hearts);

        maxHealth = Mathf.Clamp(startingMaxHealth, 0, absoluteMaxHealth);
        health = Mathf.Clamp(startingHealth, 0, maxHealth);

        EnsureHeartObjects();
        UpdateHearts();

        if (heartContainer != null)
            heartContainer.alpha = 0f;
    }

    void Update()
    {
        if (isDead)
            return;

        HandleFading();
    }

    void HandleFading()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0f)
                isFading = true;
        }

        if (isFading && heartContainer != null)
        {
            heartContainer.alpha -= Time.deltaTime / fadeDuration;

            if (heartContainer.alpha <= 0f)
            {
                heartContainer.alpha = 0f;
                isFading = false;
            }
        }
    }

    void ShowHealthBar()
    {
        if (heartContainer != null)
            heartContainer.alpha = 1f;

        isFading = false;
        hideTimer = displayDuration;
    }

    void EnsureHeartObjects()
    {
        while (heartImages.Count < maxHealth)
        {
            Image newHeart = Instantiate(heartImages[0], heartImages[0].transform.parent);
            newHeart.name = "Heart";
            newHeart.sprite = emptyHeart;
            newHeart.gameObject.SetActive(true);
            newHeart.transform.localScale = Vector3.one;
            newHeart.transform.SetAsLastSibling();
            heartImages.Add(newHeart);
        }
    }

    public void UpdateHearts()
    {
        maxHealth = Mathf.Clamp(maxHealth, 0, absoluteMaxHealth);
        health = Mathf.Clamp(health, 0, maxHealth);

        EnsureHeartObjects();

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < maxHealth)
            {
                heartImages[i].gameObject.SetActive(true);
                heartImages[i].sprite = (i < health) ? fullHeart : emptyHeart;
            }
            else
            {
                heartImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(int amount, bool bypassInvulnerabilityCheck = false)
{
    if (isDead)
        return;

    PlayerController playerController = GetComponent<PlayerController>();

    // Block outside damage during i-frames,
    // unless this call is the one that is intentionally applying the hit.
    if (!bypassInvulnerabilityCheck &&
        playerController != null &&
        playerController.IsInvulnerable())
    {
        return;
    }

    health -= amount;
    health = Mathf.Clamp(health, 0, maxHealth);

    UpdateHearts();
    ShowHealthBar();

    if (health <= 0)
        Die();
}

    public void Heal(int amount)
    {
        if (isDead)
            return;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHearts();
        ShowHealthBar();
    }

    public void AddHeartUpgrade(int amount)
    {
        if (isDead)
            return;

        for (int i = 0; i < amount; i++)
        {
            if (health < maxHealth)
            {
                health += 1;
            }
            else if (maxHealth < absoluteMaxHealth)
            {
                maxHealth += 1;
                EnsureHeartObjects();
                health = maxHealth;
            }
        }

        maxHealth = Mathf.Clamp(maxHealth, 0, absoluteMaxHealth);
        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHearts();
        ShowHealthBar();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
            playerController.HandleDeath();

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
            playerCollider.enabled = false;
        Shooting shooting = GetComponentInChildren<Shooting>();
        if (shooting != null)
            shooting.enabled = false;
            
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
        else
            Time.timeScale = 0f;
    }

    public bool IsDead()
    {
        return isDead;
    }
}