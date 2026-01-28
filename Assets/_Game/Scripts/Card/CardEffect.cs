using System;
using UnityEngine;

[Serializable]
public class CardEffect
{
    [SerializeField] CardEffectType effectType;

    public CardEffectType EffectType => effectType;
}
