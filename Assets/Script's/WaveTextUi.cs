using System.Collections;
using TMPro;
using UnityEngine;

public class WaveTextUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Timing")]
    [SerializeField] private float fadeinTime = 0.8f;
    [SerializeField] private float visibleTime = 1.5f;
    [SerializeField] private float fadeTime = 0.8f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (waveText == null)
            waveText = GetComponent<TextMeshProUGUI>();

        if (waveText != null)
        {
            waveText.text = "Wave";
            SetAlpha(0f);
        }
    }

    public void ShowWave(int waveNumber)
    {
        if (waveText == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowWaveRoutine(waveNumber));
    }

    private IEnumerator ShowWaveRoutine(int waveNumber)
    {
        waveText.text = "Wave " + waveNumber;
        
        float elapsed = 0f;
        SetAlpha(0f);

        while (elapsed < fadeinTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeinTime;
            SetAlpha(Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        yield return new WaitForSeconds(visibleTime);

         elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            SetAlpha(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = waveText.color;
        c.a = alpha;
        waveText.color = c;
    }
}