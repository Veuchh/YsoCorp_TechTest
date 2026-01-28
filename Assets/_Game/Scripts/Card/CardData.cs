using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] LocalizedString cardName;
    [SerializeField] Sprite cardImage;
    [SerializeField] CardAttributes cardAttributes;

    [Header("Card valid clicks")]
    [SerializeField] ValidTileCheckType validTileCheckType;

    [ShowIf(nameof(validTileCheckType), ValidTileCheckType.AnyInCircularRangeNoEnemy)]
    [SerializeField]
    [AllowNesting]
    [Tooltip("The range of movement you are able to move by playing this card (not accounting for diagonals)")]
    Vector2Int circularRangeNoEnemy = Vector2Int.one; 
    
    [ShowIf(nameof(validTileCheckType), ValidTileCheckType.AnyInCircularRange)]
    [SerializeField]
    [AllowNesting]
    [Tooltip("The range of movement you are able to move by playing this card (not accounting for diagonals)")]
    Vector2Int circularRange = Vector2Int.one;

    [Header("Card Effects")]
    [SerializeField] List<CardEffect> cardEffects;

    public LocalizedString CardName => cardName;
    public Sprite CardImage => cardImage;
    public CardAttributes CardAttributes => cardAttributes;
    public List<CardEffect> CardEffects => cardEffects;
    public ValidTileCheckType ValidTileCheckType => validTileCheckType;
    public Vector2Int CircularRangeNoEnemy => circularRangeNoEnemy;
    public Vector2Int CircularRange => circularRange;

    public CardEffect GetEffectByType(CardEffectType effectType)
    {
        foreach (CardEffect effect in cardEffects)
        {
            if (effect.EffectType == effectType)
            {
                return effect;
            }
        }
        return null;
    }
}
