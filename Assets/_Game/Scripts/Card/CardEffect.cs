using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class CardEffect
{
    [SerializeField] CardEffectType effectType;

    public CardEffectType EffectType => effectType;

    [ShowIf(nameof(EffectType), CardEffectType.TriggerPlayerAnimation)]
    [SerializeField]
    [AllowNesting]
    [Tooltip("The ID of the triggered animation.")]
    PlayerAnimations animationID;

    public PlayerAnimations AnimationID => animationID;
}
