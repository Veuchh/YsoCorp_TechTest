using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;

    OngoingLevelData ongoingLevelData;
    CardSelectionService cardSelectionService;

    public static LevelHandler Instance { get; private set; }

    public UnityEvent OnNewCardSelected;

    public UnityEvent<LevelData, CardSelectionService> OnLevelStarted;
    public UnityEvent OnGridGenerated;
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

        cardSelectionService = new CardSelectionService();

        cardSelectionService.OnCardSelected.AddListener(SelectCard);
        cardSelectionService.OnCardDeselected.AddListener(DeselectCard);

        OnLevelStarted?.Invoke(data, cardSelectionService);
        OnNewCardSelected?.Invoke();
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

        cardSelectionService.PlaySelectedCard();
    }
}
