using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;

    private bool gameOverActive = false;

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

        // If card selection dimmed the audio before death,
        // restore full volume first, then hard-pause the sound.
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
        ResetGameOverAndAudioState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToScene(string sceneName)
    {
        ResetGameOverAndAudioState();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitToMainMenu()
    {
        ResetGameOverAndAudioState();
        SceneManager.LoadScene("Main_Menu");
    }

    public void QuitGame()
    {
        ResetGameOverAndAudioState();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void ResetGameOverAndAudioState()
    {
        gameOverActive = false;
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