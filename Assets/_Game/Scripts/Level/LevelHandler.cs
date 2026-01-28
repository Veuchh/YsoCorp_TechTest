using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;

    OngoingLevelData ongoingLevelData;
    CardDrawAndSelectService cardDrawSelectionService;

    public static LevelHandler Instance { get; private set; }

    public UnityEvent OnNewCardSelected;

    public UnityEvent<LevelData, CardDrawAndSelectService> OnLevelStarted;
    public UnityEvent OnGridGenerated;
    public UnityEvent OnNewPlayedCardList;
    public UnityEvent<PlayedCard> OnCardUndone;
    public UnityEvent<Tile> OnTileClicked;

    public OngoingLevelData OngoingLevelData => ongoingLevelData;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to initialize an Intance of {nameof(LevelHandler)}, but one already existed. Aborting.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public async void StartLevel(LevelData data)
    {
        //Delaying a few frames to make sure everything is properly initialized
        await UniTask.DelayFrame(3);

        ongoingLevelData = levelGenerator.GenerateLevel(data);

        OnGridGenerated?.Invoke();

        cardDrawSelectionService = new CardDrawAndSelectService();

        cardDrawSelectionService.OnCardSelected.AddListener(SelectCard);
        cardDrawSelectionService.OnCardDeselected.AddListener(DeselectCard);

        OnLevelStarted?.Invoke(data, cardDrawSelectionService);
        OnNewCardSelected?.Invoke();
        OnNewPlayedCardList?.Invoke();
    }

    public void SelectCard(CardDispenser _, CardData selectedCard)
    {
        ongoingLevelData.CurrentlySelectedCard = selectedCard;
        OnNewCardSelected?.Invoke();
    }

    public void DeselectCard(CardDispenser _)
    {
        ongoingLevelData.CurrentlySelectedCard = null;
        OnNewCardSelected?.Invoke();
    }

    public void ClickTile(Tile clickedTile)
    {
        OnTileClicked?.Invoke(clickedTile);
    }

    public void PlayCardOnTile(CardData currentlySelectedCard, Vector2Int clickedTileCoord)
    {
        //Add card to list for undos and final visualization
        PlayedCard playedCard = new PlayedCard(
            cardData: currentlySelectedCard,
            playerPosOnStartPlayCard: ongoingLevelData.CurrentPlayerPreviewPosition,
            clickedTileCoord: clickedTileCoord,
            originDispenser: cardDrawSelectionService.SelectedDispenser);

        ongoingLevelData.AddPlayedCard(playedCard);
        cardDrawSelectionService.PlaySelectedCard();

        OnNewPlayedCardList?.Invoke();
    }

    public void TryUndo()
    {
        PlayedCard undoneCard = ongoingLevelData.TryGetAndRemoveUndoCard();

        if (undoneCard != null)
        {
            OnCardUndone?.Invoke(undoneCard);
        }

        OnNewPlayedCardList?.Invoke();
        cardDrawSelectionService.Undo(undoneCard);
    }
}
