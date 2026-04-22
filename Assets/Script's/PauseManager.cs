using System;
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
    [SerializeField] private UIButtonAudio uiAudio;

    [Header("References")]
    [SerializeField] private GameOverManager gameOverManager;
    private PauseSource activePauseSources = PauseSource.None;

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
        // Safety reset in case audio was left paused by another scene/state
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
            // Do not allow pause menu toggling while card selection is active
            if (IsCardSelectionPaused)
                return;

            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (IsGameOverActive())
            return;

        if (IsPauseMenuPaused)
            ResumeFromSource(PauseSource.PauseMenu);
        else
            PauseFromSource(PauseSource.PauseMenu);
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

        // Pause audio only for hard pauses.
        // CardSelection should only dim volume through CardManager, not fully pause audio.
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
        if (IsGameOverActive())
            return;

        if (uiAudio != null)
            uiAudio.PlayClick();

        PauseFromSource(PauseSource.PauseMenu);
    }

    public void ResumeGame()
    {
        if (IsGameOverActive())
            return;

        ResumeFromSource(PauseSource.PauseMenu);
    }

    public void RestartGame()
    {
        ResetPauseAndAudioState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        ResetPauseAndAudioState();
        SceneManager.LoadScene("Main_Menu");
    }

    public void QuitGame()
    {
        ResetPauseAndAudioState();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void ResetPauseAndAudioState()
    {
        activePauseSources = PauseSource.None;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }
}