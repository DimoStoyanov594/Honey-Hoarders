using UnityEngine;

public class UIButtonSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadSceneWithClick()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is empty on " + gameObject.name);
            return;
        }

        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClickAndLoadScene(sceneName);
    }
}