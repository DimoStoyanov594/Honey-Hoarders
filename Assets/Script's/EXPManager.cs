using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EXPManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float epxGrowthMultiplier = 1.2f;

    public Slider expSlider;
    public TMP_Text currentLevelText;

    private CardManager cardManager;

    private void Awake()
    {
        cardManager = GetComponent<CardManager>();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2);
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * epxGrowthMultiplier);
        cardManager.OnLevelUp();
    }

    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        currentLevelText.text = "Level: " + level;
    }
}