using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EXPManager : MonoBehaviour
{
    public int level = 0;
    public int currentExp = 0;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f;

    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private CardManager cardManager;

    private void Start()
    {
        UpdateUI();
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToLevel)
        {
            currentExp -= expToLevel;
            level++;
            expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);

            if (cardManager != null)
                cardManager.OnLevelUp();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToLevel;
            expSlider.value = currentExp;
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = "Level: " + level;
        }
    }
}