using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite artwork;
    public CardEffectType effectType;
    public int value;
    public bool isUnique;
}

public enum CardEffectType
{
    IncreaseMaxHealth,
    IncreaseDamage,
    IncreaseSpeed,
    ReduceExpRequired,
    SummonCompanion,
}