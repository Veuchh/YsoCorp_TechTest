using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    [SerializeField] GameObject planificationPhaseUI;
    [SerializeField] CanvasGroup visualizationTransitionCanvasGroup;
    [SerializeField] float visualizationTransitionDuration = 1f;

    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnLevelStarted.AddListener(OnLevelStarted);
        LevelHandler.Instance.OnNewPlayedCardList.AddListener(RefreshButtonsState);
        LevelHandler.Instance.OnStartVisualization.AddListener(PlayVisualizationTransition);

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
        planificationPhaseUI.SetActive(true);
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
            LevelHandler.Instance.OngoingLevelData.LevelStates.Count != 0;
        playButton.interactable =
            LevelHandler.Instance.OngoingLevelData.LevelStates.Count != 0;
    }

    private void OnUndoClicked()
    {
        LevelHandler.Instance.TryUndo();
    }

    private void OnPlayClicked()
    {
        LevelHandler.Instance.TryPlaySequence();
    }

    private void PlayVisualizationTransition()
    {
        Sequence visualisationSequence = DOTween.Sequence();

        visualisationSequence.AppendCallback(() => visualizationTransitionCanvasGroup.blocksRaycasts = true);
        visualisationSequence.AppendCallback(() => planificationPhaseUI.SetActive(false));
        visualisationSequence.Append(visualizationTransitionCanvasGroup.DOFade(1, visualizationTransitionDuration / 4));
        visualisationSequence.AppendInterval(visualizationTransitionDuration / 2);
        visualisationSequence.Append(visualizationTransitionCanvasGroup.DOFade(0, visualizationTransitionDuration / 4));
        visualisationSequence.AppendCallback(() => visualizationTransitionCanvasGroup.blocksRaycasts = false);
    }
}
