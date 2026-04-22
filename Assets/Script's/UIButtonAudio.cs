using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonAudio : MonoBehaviour
{
    public static UIButtonAudio Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private bool isBusyTransitioning = false;

    private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    if (audioSource == null)
        audioSource = GetComponent<AudioSource>();

    if (audioSource != null)
        audioSource.ignoreListenerPause = true;
}

    public void PlayClick()
    {
        if (audioSource == null || clickSound == null)
            return;

        audioSource.PlayOneShot(clickSound);
    }

    public void PlayClickAndLoadScene(string sceneName)
    {
        if (isBusyTransitioning)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("PlayClickAndLoadScene was called with an empty scene name.");
            return;
        }

        StartCoroutine(PlayClickThenLoadScene(sceneName));
    }

    public void PlayClickAndQuit()
    {
        if (isBusyTransitioning)
            return;

        StartCoroutine(PlayClickThenQuit());
    }

    private IEnumerator PlayClickThenLoadScene(string sceneName)
    {
        isBusyTransitioning = true;

        float waitTime = PlayClickAndGetLength();
        yield return new WaitForSecondsRealtime(waitTime);

        SceneManager.LoadScene(sceneName);
        isBusyTransitioning = false;
    }

    private IEnumerator PlayClickThenQuit()
    {
        isBusyTransitioning = true;

        float waitTime = PlayClickAndGetLength();
        yield return new WaitForSecondsRealtime(waitTime);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private float PlayClickAndGetLength()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            return clickSound.length;
        }

        return 0f;
    }
}