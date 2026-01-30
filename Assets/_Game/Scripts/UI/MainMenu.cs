using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<LevelData> levels;
    [SerializeField] List<GameObject> levelButtons;
    [SerializeField] LevelButton levelButtonPrefab;
    [SerializeField] Transform levelButtonsParent;

    int currentlyPlayedLevelIndex = -1;
    int currentMaxLevelIndex = 0;

    private async void Awake()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            LevelData levelData = levels[i];
            LevelButton newButton = Instantiate(levelButtonPrefab, levelButtonsParent);
            newButton.Initialize(levelData, i+1);
            newButton.OnClick.AddListener(OnLevelButtonClicked);

            levelButtons.Add(newButton.gameObject);

            if (i != 0)
                newButton.gameObject.SetActive(false);
        }

        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnLevelWon.AddListener(TryUnlockNextLevel);
        LevelHandler.Instance.OnToggleMainMenu.AddListener(ToggleMenu);

        TryUnlockNextLevel();
    }

    private void TryUnlockNextLevel()
    {
        if (currentMaxLevelIndex <= currentlyPlayedLevelIndex)
        {
            currentMaxLevelIndex = currentlyPlayedLevelIndex + 1;
        }

        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].SetActive(i <= currentMaxLevelIndex);
        }
    }

    private void ToggleMenu(bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
        canvasGroup.interactable = toggle;
    }

    private void OnLevelButtonClicked(LevelData levelData)
    {
        currentlyPlayedLevelIndex = levels.IndexOf(levelData);

        LevelHandler.Instance.StartLevel(levelData);
    }
}
