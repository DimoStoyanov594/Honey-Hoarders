using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClickAndLoadScene("Game-Scene");
    }

    public void SkinsButton()
    {
        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClickAndLoadScene("Skin_Select");
    }

    public void QuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            if (UIButtonAudio.Instance != null)
        UIButtonAudio.Instance.PlayClickAndQuit();
        #endif
    }
}