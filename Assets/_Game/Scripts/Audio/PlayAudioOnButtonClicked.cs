using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayAudioOnButtonClicked : MonoBehaviour
{
    [SerializeField] AudioData audioData;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayAudio);
    }

    private void PlayAudio()
    {
        AudioManager.Instance.PlayAudioData(audioData);
    }
}
