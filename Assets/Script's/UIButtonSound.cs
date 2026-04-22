using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private UIButtonAudio uiAudio;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (uiAudio != null)
            uiAudio.PlayClick();
    }
}