using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        // Loads game with whatever skin was last equipped — no need to touch PlayerPrefs
        SceneManager.LoadScene("Game-Scene");
    }

    public void SkinsButton()
    {
        SceneManager.LoadScene("Skin_Select");
    }

    public void QuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}