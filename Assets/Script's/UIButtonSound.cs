using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (UIButtonAudio.Instance != null)
            UIButtonAudio.Instance.PlayClick();
    }
}