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
}