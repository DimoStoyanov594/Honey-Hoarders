using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;

    [Header("UI Audio")]
    [SerializeField] private float transitionClickDelay = 0.1f;

    private bool gameOverActive = false;
    private bool isTransitioning = false;

    private void Start()
    {
        // Safety reset in case previous scene/state left audio modified
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverActive)
            return;

        gameOverActive = true;
        isTransitioning = false;

        // Restore full volume first in case another system dimmed it,
        // then hard-pause world audio.
        AudioListener.volume = 1f;
        AudioListener.pause = true;

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 1f;
            gameOverCanvasGroup.interactable = true;
            gameOverCanvasGroup.blocksRaycasts = true;
        }
    }

    public bool IsGameOverActive()
    {
        return gameOverActive;
    }

    public void RestartGame()
    {
        if (!gameOverActive || isTransitioning)
            return;

        StartCoroutine(RestartGameRoutine());
    }

    public void QuitToScene(string sceneName)
    {
        if (!gameOverActive || isTransitioning)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("QuitToScene was called with an empty scene name.");
            return;
        }

        StartCoroutine(QuitToSceneRoutine(sceneName));
    }

    public void QuitToMainMenu()
    {
        if (!gameOverActive || isTransitioning)
            return;

        StartCoroutine(QuitToMainMenuRoutine());
    }

    public void QuitGame()
    {
        if (!gameOverActive || isTransitioning)
            return;

        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetGameOverAndAudioState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator QuitToSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetGameOverAndAudioState();
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitToMainMenuRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetGameOverAndAudioState();
        SceneManager.LoadScene("Main_Menu");
    }

    private IEnumerator QuitGameRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetGameOverAndAudioState();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayUIClick()
    {
        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClick();
    }

    private void ResetGameOverAndAudioState()
    {
        gameOverActive = false;
        isTransitioning = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}