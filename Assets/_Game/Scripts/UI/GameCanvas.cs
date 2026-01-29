using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] CanvasGroup globalCanvasGroup;

    [Header("Previsualization UI")]
    [SerializeField] CardDispenser cardDispenserPrefab;
    [SerializeField] SquareRowLayout squareRowLayout;
    [SerializeField] Transform cardDispensersParent;
    [SerializeField] Button undoButton;
    [SerializeField] Button playButton;
    [SerializeField] GameObject planificationPhaseUI;

    [Header("Visualisation UI")]
    [SerializeField] CanvasGroup visualizationTransitionCanvasGroup;
    [SerializeField] float visualizationTransitionDuration = 1f;

    [Header("Victory Screen")]
    [SerializeField] CanvasGroup victoryScreenCanvasGroup;
    [SerializeField] Button victoryScreenMainMenuButton;

    [Header("Defeat Screen")]
    [SerializeField] CanvasGroup defeatScreenCanvasGroup;
    [SerializeField] Button defeatScreenMainMenuButton;
    [SerializeField] Button defeatScreenRetryButton;
    [SerializeField] LocalizeStringEvent defeatScreenDefeatReasonLabel;
    [SerializeField] LocalizedString defeatReasonEnemyRemaining;
    [SerializeField] LocalizedString defeatReasonSameTile;
    [SerializeField] LocalizedString defeatReasonBottom;

    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        //Level handler events
        LevelHandler.Instance.OnLevelStarted.AddListener(OnLevelStarted);
        LevelHandler.Instance.OnNewPlayedCardList.AddListener(RefreshButtonsState);
        LevelHandler.Instance.OnStartVisualization.AddListener(PlayVisualizationTransition);
        LevelHandler.Instance.OnLevelWon.AddListener(ShowLevelWonScreen);
        LevelHandler.Instance.OnLevelLost.AddListener(ShowLevelLostScreen);
        LevelHandler.Instance.OnToggleMainMenu.AddListener(OnToggleMainMenu);


        //buttons events
        undoButton.onClick.AddListener(OnUndoClicked);
        playButton.onClick.AddListener(OnPlayClicked);

        victoryScreenMainMenuButton.onClick.AddListener(ReturnToMenuClicked);

        defeatScreenMainMenuButton.onClick.AddListener(ReturnToMenuClicked);
        defeatScreenRetryButton.onClick.AddListener(RetryLevelClicked);

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

        ToggleCanvasGroup(globalCanvasGroup, true);
        ToggleCanvasGroup(victoryScreenCanvasGroup, false);
        ToggleCanvasGroup(defeatScreenCanvasGroup, false);

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

    private void ShowLevelWonScreen()
    {
        ToggleCanvasGroup(victoryScreenCanvasGroup, true);
    }

    private void ShowLevelLostScreen(DefeatReason defeatReason, EnemyEntity _)
    {
        ToggleCanvasGroup(defeatScreenCanvasGroup, true);
        defeatScreenDefeatReasonLabel.StringReference =
            GetLocalizedStringFromDefeatReason(defeatReason);
    }

    private void OnToggleMainMenu(bool isMainMenuOn)
    {
        if (!isMainMenuOn)
            return;

        ToggleCanvasGroup(globalCanvasGroup, false);
    }

    private void ToggleCanvasGroup(CanvasGroup canvasGroup, bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
        canvasGroup.interactable = toggle;
    }

    private LocalizedString GetLocalizedStringFromDefeatReason(DefeatReason defeatReason)
    {
        switch (defeatReason)
        {
            case DefeatReason.SameTileAsEnemy:
                return defeatReasonSameTile;
            case DefeatReason.EnemyReachedBottom:
                return defeatReasonBottom;
            case DefeatReason.EnemyAliveAfterVisualization:
                return defeatReasonEnemyRemaining;
        }

        return null;
    }

    private void ReturnToMenuClicked()
    {
        LevelHandler.Instance.ReturnToMainMenu();
    }

    private void RetryLevelClicked()
    {
        LevelHandler.Instance.RestartLevel();
    }
}
