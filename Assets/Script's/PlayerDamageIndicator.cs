using System.Collections;
using UnityEngine;

public class PlayerDamageIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Blink Settings")]
    [SerializeField] private float blinkDuration = 0.75f;
    [SerializeField] private float blinkInterval = 0.1f;

    private Coroutine blinkRoutine;

    private void Awake()
    {
          if (spriteRenderer == null)
          spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ShowDamageBlink()
    {

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

     private IEnumerator BlinkRoutine()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval * 2f;
        }

        spriteRenderer.enabled = true;
        blinkRoutine = null;
    }
}