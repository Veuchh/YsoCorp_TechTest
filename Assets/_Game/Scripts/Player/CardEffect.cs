using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class CardEffect
{
    [SerializeField] CardEffectType effectType;

    public CardEffectType EffectType => effectType;
    public Vector2Int ChoseMoveRange => choseMoveRange;

    //EffectType == CardEffectType.ChoseMovePosition
    [ShowIf(nameof(effectType), CardEffectType.ChoseMovePosition)]
    [SerializeField]
    [AllowNesting]
    [Tooltip("The range of movement you are able to move by playing this card (not accounting for diagonals)")]
    Vector2Int choseMoveRange = Vector2Int.one;
}
