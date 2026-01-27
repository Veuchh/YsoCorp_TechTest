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
    [SerializeField] List<CardEffect> cardEffects;

    public LocalizedString CardName => cardName;
    public Sprite CardImage => cardImage;
    public CardAttributes CardAttributes => cardAttributes;
    public List<CardEffect> CardEffects => cardEffects;

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
