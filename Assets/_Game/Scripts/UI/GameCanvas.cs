using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] CardDispenser cardDispenserPrefab;
    [SerializeField] SquareRowLayout squareRowLayout;
    [SerializeField] Transform cardDispensersParent;
    [SerializeField] Button undoButton;
    [SerializeField] Button playButton;

    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnLevelStarted.AddListener(OnLevelStarted);
        LevelHandler.Instance.OnNewPlayedCardList.AddListener(RefreshButtonsState);

        undoButton.onClick.AddListener(OnUndoClicked);
        playButton.onClick.AddListener(OnPlayClicked);
    }

    void TryClearUI()
    {
        for (int i = cardDispensersParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cardDispensersParent.GetChild(i).gameObject);
        }
    }

    private void OnLevelStarted(LevelData leveldata, CardDrawAndSelectService cardSelectionService)
    {
        TryClearUI();

        foreach (CardAttributes cardDispenserAttributes in leveldata.cardDispenserAttributes)
        {
            CardDispenser newCardDispenser = Instantiate(cardDispenserPrefab, cardDispensersParent);
            newCardDispenser.Initialize(cardDispenserAttributes, cardSelectionService);
        }

        squareRowLayout.Initialize();
    }

    private void RefreshButtonsState()
    {
        undoButton.interactable =
            LevelHandler.Instance.OngoingLevelData.PlayedCards.Count != 0;
    }

    private void OnUndoClicked()
    {
        LevelHandler.Instance.TryUndo();
    }

    private void OnPlayClicked()
    {

    }
}
