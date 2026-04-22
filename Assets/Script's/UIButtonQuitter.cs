using UnityEngine;

public class UIButtonQuitter : MonoBehaviour
{
    public void QuitWithClick()
    {
        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClickAndQuit();
    }
}