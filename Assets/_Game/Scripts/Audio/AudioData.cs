using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField]
    List<AudioClip> clips;
    [SerializeField]
    [MinMaxSlider(0, 2)]
    Vector2 randomPitchMinMax = Vector2.one;

    [SerializeField]
    [MinMaxSlider(0, 1)]
    Vector2 randomVolumeMinMax = Vector2.one;

    public AudioClip GetClip => clips[Random.Range(0, clips.Count)];
    public float GetPitch => Random.Range(randomPitchMinMax.x, randomPitchMinMax.y);
    public float GetVolume => Random.Range(randomVolumeMinMax.x, randomVolumeMinMax.y);
}
