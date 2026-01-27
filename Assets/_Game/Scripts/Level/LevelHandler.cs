using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Player playerPrefab;

    Player player;
    OngoingLevelData ongoingLevelData;
    CardSelectionService cardSelectionService;

    public static LevelHandler Instance { get; private set; }

    public UnityEvent OnNewCardSelected;

    public UnityEvent<LevelData, CardSelectionService> OnLevelStarted;

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

    public async void StartLevel(LevelData data)
    {
        //Delaying a few frames to make sure everything is properly initialized
        await UniTask.DelayFrame(3);

        ClearLevel();

        ongoingLevelData = levelGenerator.GenerateLevel(data);

        cameraManager.SetCameraPosition(ongoingLevelData.Tiles);

        player = Instantiate(playerPrefab);
        player.transform.position = ongoingLevelData.GetTileWorldCoordinate(ongoingLevelData.LevelData.PlayerStartPos);


        cardSelectionService = new CardSelectionService();

        cardSelectionService.OnCardSelected.AddListener(SelectCard);
        cardSelectionService.OnCardDeselected.AddListener(DeselectCard);

        OnLevelStarted?.Invoke(data, cardSelectionService);
        OnNewCardSelected?.Invoke();
    }

    public void SelectCard(CardDispenser _, CardData selectedCard)
    {
        ongoingLevelData.SelectedCard = selectedCard;
        OnNewCardSelected?.Invoke();
    }

    public void DeselectCard(CardDispenser _)
    {
        ongoingLevelData.SelectedCard = null;
        OnNewCardSelected?.Invoke();
    }

    void ClearLevel()
    {
        if (player)
        {
            Destroy(player.gameObject);
        }
    }
}
