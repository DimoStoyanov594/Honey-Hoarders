using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;

    private CardData data;
    private CardManager manager;

    public void Setup(CardData cardData, CardManager cardManager)
    {
        data = cardData;
        manager = cardManager;

        nameText.text = data.cardName;
        descriptionText.text = data.description;

        if (data.artwork != null)
            artworkImage.sprite = data.artwork;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.SelectCard(data);
    }
}