using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public UnityEvent<LevelData> OnClick;

    [SerializeField] TextMeshProUGUI levelLabel;

    LevelData levelData;

    public void Initialize(LevelData levelData, int levelID)
    {
        this.levelData = levelData;
        levelLabel.text = levelID.ToString();
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        OnClick?.Invoke(levelData);
    }
}
