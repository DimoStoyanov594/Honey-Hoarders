using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Flags]
    public enum PauseSource
    {
        None = 0,
        PauseMenu = 1 << 0,
        CardSelection = 1 << 1
    }

    [Header("UI")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private CanvasGroup pauseCanvasGroup;

    [Header("References")]
    [SerializeField] private GameOverManager gameOverManager;

    [Header("UI Audio")]
    [SerializeField] private float transitionClickDelay = 0.1f;

    private PauseSource activePauseSources = PauseSource.None;
    private bool isTransitioning = false;

    public static event Action<bool> OnPauseStateChanged;

    public bool IsPaused => activePauseSources != PauseSource.None;
    public bool IsPauseMenuPaused => HasPauseSource(PauseSource.PauseMenu);
    public bool IsCardSelectionPaused => HasPauseSource(PauseSource.CardSelection);

    private bool IsGameOverActive()
    {
        return gameOverManager != null && gameOverManager.IsGameOverActive();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AudioListener.pause = false;

        HidePauseMenuUI();
        RefreshPauseState();
    }

    private void Update()
    {
        if (IsGameOverActive())
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsCardSelectionPaused)
                return;

            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (IsGameOverActive() || isTransitioning)
            return;

        if (IsPauseMenuPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseFromSource(PauseSource source)
    {
        activePauseSources |= source;

        if (source == PauseSource.PauseMenu)
            ShowPauseMenuUI();

        RefreshPauseState();
    }

    public void ResumeFromSource(PauseSource source)
    {
        activePauseSources &= ~source;

        if (source == PauseSource.PauseMenu)
            HidePauseMenuUI();

        RefreshPauseState();
    }

    public bool HasPauseSource(PauseSource source)
    {
        return (activePauseSources & source) != 0;
    }

    private void RefreshPauseState()
    {
        bool paused = IsPaused;

        Time.timeScale = paused ? 0f : 1f;

        // Only hard-pause audio for the pause menu.
        // Card selection should not fully pause audio.
        bool shouldPauseAudio = HasPauseSource(PauseSource.PauseMenu);
        AudioListener.pause = shouldPauseAudio;

        OnPauseStateChanged?.Invoke(paused);
    }

    private void ShowPauseMenuUI()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = true;
            pauseCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HidePauseMenuUI()
    {
        if (pauseButton != null)
            pauseButton.SetActive(true);

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 0f;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        if (IsGameOverActive() || isTransitioning)
            return;

        PlayUIClick();
        PauseFromSource(PauseSource.PauseMenu);
    }

    public void ResumeGame()
    {
        if (IsGameOverActive() || isTransitioning)
            return;

        PlayUIClick();
        ResumeFromSource(PauseSource.PauseMenu);
    }

    public void RestartGame()
    {
        if (isTransitioning)
            return;

        StartCoroutine(RestartGameRoutine());
    }

    public void QuitToMainMenu()
    {
        if (isTransitioning)
            return;

        StartCoroutine(QuitToMainMenuRoutine());
    }

    public void QuitGame()
    {
        if (isTransitioning)
            return;

        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetPauseAndAudioState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator QuitToMainMenuRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetPauseAndAudioState();
        SceneManager.LoadScene("Main_Menu");
    }

    private IEnumerator QuitGameRoutine()
    {
        isTransitioning = true;

        PlayUIClick();
        yield return new WaitForSecondsRealtime(transitionClickDelay);

        ResetPauseAndAudioState();

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

    private void ResetPauseAndAudioState()
    {
        activePauseSources = PauseSource.None;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 1f;
        HidePauseMenuUI();
    }
}