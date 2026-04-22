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
    [SerializeField] private Shooting shooting;

    [Header("Upgrade Values")]
    [SerializeField] private int damageUpgradeAmount = 1;
    [SerializeField] private float speedUpgradeAmount = 0.1f;
    [SerializeField] private float fireRateUpgradeAmount = 0.05f;
    [SerializeField] private float minimumTimeBetweenShots = 0.05f;

    [Header("Audio")]
    [SerializeField, Range(0f, 1f)] private float cardScreenVolume = 0.25f;

    private float previousAudioVolume = 1f;
    private bool cardAudioDimmed = false;
    private GameObject activeCompanion;

    [SerializeField] private EXPManager expManager;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private PlayerController playerController;
    private List<CardData> usedUniqueCards = new List<CardData>();

    private bool isShowingCards = false;
    private int queuedLevelUps = 0;
    private bool cardAlreadyChosen = false;

    private void OnDisable()
    {
        RestoreAudioAfterCardScreen();
    }
    private void Awake()
    {
        cardSelectionPanel.SetActive(false);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }
    }
    private void LowerAudioForCardScreen()
    {
        if (cardAudioDimmed)
            return;

        previousAudioVolume = AudioListener.volume;
        AudioListener.volume = cardScreenVolume;
        cardAudioDimmed = true;
    }

    private void RestoreAudioAfterCardScreen()
    {
        if (!cardAudioDimmed)
            return;

        AudioListener.volume = previousAudioVolume;
        cardAudioDimmed = false;
    }
    public void OnLevelUp()
    {
        queuedLevelUps++;

        if (!isShowingCards)
            StartCoroutine(ShowCardsRoutine());
    }

    private IEnumerator ShowCardsRoutine()
    {
        isShowingCards = true;

        while (queuedLevelUps > 0)
        {
            queuedLevelUps--;
            cardAlreadyChosen = false;

            cardSelectionPanel.SetActive(true);

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

            if (PauseManager.Instance != null)
            PauseManager.Instance.PauseFromSource(PauseManager.PauseSource.CardSelection);

            LowerAudioForCardScreen();

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / fadeDuration;

                if (overlayImage != null)
                {
                    Color c = overlayImage.color;
                    c.a = Mathf.Lerp(0f, 0.75f, t);
                    overlayImage.color = c;
                }

                yield return null;
            }

            yield return new WaitUntil(() => !cardSelectionPanel.activeSelf);
        }

        isShowingCards = false;
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
        if (cardAlreadyChosen)
            return;

        cardAlreadyChosen = true;

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

            yield return null;
        }

        cardSelectionPanel.SetActive(false);

        RestoreAudioAfterCardScreen();

        if (PauseManager.Instance != null)
            PauseManager.Instance.ResumeFromSource(PauseManager.PauseSource.CardSelection);

        ApplyCardEffect(card);
    }

    private void ApplyCardEffect(CardData card)
    {
        switch (card.effectType)
        {
            case CardEffectType.IncreaseMaxHealth:
                if (healthManager != null)
                    healthManager.AddHeartUpgrade(card.value);
                break;

            case CardEffectType.IncreaseDamage:
                if (shooting != null)
                    shooting.SetBulletDamage(shooting.GetBulletDamage() + damageUpgradeAmount);
                break;

            case CardEffectType.IncreaseSpeed:
                if (playerController != null)
                    playerController.moveSpeed += speedUpgradeAmount;
                break;

            case CardEffectType.ReduceExpRequired:
                if (expManager != null)
                    expManager.expToLevel = Mathf.Max(1, expManager.expToLevel - card.value);
                break;

            case CardEffectType.SummonCompanion:
                if (activeCompanion == null && companionPrefab != null)
                {
                    activeCompanion = Instantiate(
                        companionPrefab,
                        player.position + Vector3.left * 1.5f,
                        Quaternion.identity
                    );

                    SpriteRenderer companionRenderer = activeCompanion.GetComponent<SpriteRenderer>();
                    SpriteRenderer playerRenderer = player.GetComponentInChildren<SpriteRenderer>();

                    if (companionRenderer != null && playerRenderer != null)
                    {
                        companionRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                        companionRenderer.sortingOrder = playerRenderer.sortingOrder;
                    }

                    CompanionAI companionAI = activeCompanion.GetComponent<CompanionAI>();
                    if (companionAI != null)
                        companionAI.Initialize(player);
                }
                break;

            case CardEffectType.IncreaseFireRate:
                if (shooting != null)
                {
                    float newTime = shooting.GetTimeBetweenFiring() - fireRateUpgradeAmount;
                    shooting.SetTimeBetweenFiring(Mathf.Max(minimumTimeBetweenShots, newTime));
                }
                break;
        }
    }
}