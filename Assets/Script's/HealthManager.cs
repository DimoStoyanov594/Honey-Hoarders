using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 3;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public CanvasGroup heartContainer; 
    public float displayDuration = 2f;  // How long to show after being hit
    public float fadeDuration = 1f;     // How long the fade-out takes

    private float hideTimer = 0f;
    private bool isFading = false;

    void Start()
    {
        // Hide health bar at the start
        heartContainer.alpha = 0f;
    }

    void Update()
    {
        UpdateHearts();
        HandleFading();
    }

    void UpdateHearts()
    {
        foreach (Image img in hearts)
            img.sprite = emptyHeart;

        for (int i = 0; i < health; i++)
            hearts[i].sprite = fullHeart;
    }

    void HandleFading()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0f)
                isFading = true; // Start fading once timer runs out
        }

        if (isFading)
        {
            heartContainer.alpha -= Time.deltaTime / fadeDuration;

            if (heartContainer.alpha <= 0f)
            {
                heartContainer.alpha = 0f;
                isFading = false;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, hearts.Length);

        // Show health bar and reset the timer
        heartContainer.alpha = 1f;
        isFading = false;
        hideTimer = displayDuration;
    }
}