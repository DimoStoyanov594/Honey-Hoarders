using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cardSelectionPanel;
    [SerializeField] private CardUI[] cardSlots;
    [SerializeField] private Image overlayImage;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("Card Pool")]
    [SerializeField] private List<CardData> allCards;

    [Header("Companion")]
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private Transform player;

    private EXPManager expManager;
    private GameObject activeCompanion;
    private List<CardData> usedUniqueCards = new List<CardData>();

    private void Awake()
    {
        expManager = GetComponent<EXPManager>();
        cardSelectionPanel.SetActive(false);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0;
            overlayImage.color = c;
        }
    }

    public void OnLevelUp()
    {
        StartCoroutine(ShowCardsRoutine());
    }

    private IEnumerator ShowCardsRoutine()
    {
        cardSelectionPanel.SetActive(true);

        // Hide all slots first to clear any leftovers
        foreach (CardUI slot in cardSlots)
            slot.gameObject.SetActive(false);

        List<CardData> offered = GetRandomCards(3);

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < offered.Count)
            {
                cardSlots[i].gameObject.SetActive(true);
                cardSlots[i].Setup(offered[i], this);
            }
            else
            {
                cardSlots[i].gameObject.SetActive(false);
            }
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = Mathf.Lerp(0, 0.75f, t);
                overlayImage.color = c;
            }

            Time.timeScale = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        Time.timeScale = 0f;
    }

    private List<CardData> GetRandomCards(int count)
    {
        List<CardData> pool = new List<CardData>();
        foreach (CardData card in allCards)
        {
            if (!usedUniqueCards.Contains(card))
                pool.Add(card);
        }

        List<CardData> result = new List<CardData>();
        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return result;
    }

    public void SelectCard(CardData card)
    {
        if (card.isUnique)
            usedUniqueCards.Add(card);

        StartCoroutine(HideCardsRoutine(card));
    }

    private IEnumerator HideCardsRoutine(CardData card)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration * 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (fadeDuration * 0.5f);

            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = Mathf.Lerp(0.75f, 0f, t);
                overlayImage.color = c;
            }

            Time.timeScale = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        Time.timeScale = 1f;
        cardSelectionPanel.SetActive(false);
        ApplyCardEffect(card);
    }

    private void ApplyCardEffect(CardData card)
    {
        switch (card.effectType)
        {
            case CardEffectType.IncreaseMaxHealth:
                break;
            case CardEffectType.IncreaseDamage:
                break;
            case CardEffectType.IncreaseSpeed:
                break;
            case CardEffectType.ReduceExpRequired:
                expManager.expToLevel = Mathf.Max(1,
                    expManager.expToLevel - card.value);
                break;
            case CardEffectType.SummonCompanion:
                if (activeCompanion == null && companionPrefab != null)
                {
                    activeCompanion = Instantiate(
                        companionPrefab,
                        player.position + Vector3.left * 1.5f,
                        Quaternion.identity);

                    activeCompanion.GetComponent<CompanionAI>().Initialize(player);
                }
                break;
        }

        Debug.Log($"Applied: {card.cardName}");
    }
}