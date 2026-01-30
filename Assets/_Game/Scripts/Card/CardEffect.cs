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

    [ShowIf(nameof(EffectType), CardEffectType.PlayAudio)]
    [SerializeField]
    [AllowNesting]
    [Tooltip("The data of the played audio.")]
    AudioData audioDataToPlay;

    public AudioData AudioDataToPlay => audioDataToPlay;
}
