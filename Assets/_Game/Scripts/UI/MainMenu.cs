using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<LevelData> levels;
    [SerializeField] LevelButton levelButtonPrefab;
    [SerializeField] Transform levelButtonsParent;


    private async void Awake()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            LevelData levelData = levels[i];
            LevelButton newButton = Instantiate(levelButtonPrefab, levelButtonsParent);
            newButton.Initialize(levelData, i+1);
            newButton.OnClick.AddListener(OnLevelButtonClicked);
        }

        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnToggleMainMenu.AddListener(ToggleMenu);
    }

    private void ToggleMenu(bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
        canvasGroup.interactable = toggle;
    }

    private void OnLevelButtonClicked(LevelData levelData)
    {
        LevelHandler.Instance.StartLevel(levelData);
    }
}
