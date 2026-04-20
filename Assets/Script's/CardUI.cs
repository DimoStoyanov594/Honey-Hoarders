using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private CardData data;
    private CardManager manager;

    private bool interactable = true;
    private bool alreadyClicked = false;

    public void Setup(CardData cardData, CardManager cardManager)
    {
        data = cardData;
        manager = cardManager;

        if (nameText != null)
            nameText.text = data.cardName;

        if (descriptionText != null)
            descriptionText.text = data.description;

        if (artworkImage != null)
        {
            artworkImage.sprite = data.artwork;
            artworkImage.enabled = data.artwork != null;
        }

        alreadyClicked = false;
        SetInteractable(true);
    }

    public void SetInteractable(bool value)
    {
        interactable = value;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = value;
            canvasGroup.interactable = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable || alreadyClicked || manager == null || data == null)
            return;

        alreadyClicked = true;
        interactable = false;
        manager.SelectCard(data);
    }
}