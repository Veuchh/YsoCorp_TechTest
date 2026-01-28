using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class EntitiesHandler : MonoBehaviour
{
    public static EntitiesHandler Instance;
    [SerializeField] Player playerPrefab;

    private async void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to initialize an Intance of {nameof(EntitiesHandler)}, but one already existed. Aborting.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnGridGenerated.AddListener(OnGridGenerated);
        LevelHandler.Instance.OnCardUndone.AddListener(OnUndo);
    }

    private void OnDestroy()
    {
        Instance = null;
        LevelHandler.Instance?.OnGridGenerated.RemoveListener(OnGridGenerated);
    }

    private void OnGridGenerated()
    {
        //Spawn player
        LevelHandler.Instance.OngoingLevelData.Player =
            Instantiate(playerPrefab, transform);

        MovePlayerToTileByCoord(LevelHandler.Instance.OngoingLevelData.LevelData.PlayerStartPos);

        //spawn enemies
    }

    public void MovePlayerToTile(Tile tile)
    {
        MovePlayerToTileByCoord(LevelHandler.Instance.OngoingLevelData.GetTileGridCoord(tile));
    }

    public void MovePlayerToTileByCoord(Vector2Int tileCoord, bool reverseRotation = false)
    {
        LevelHandler.Instance.OngoingLevelData.CurrentPlayerPreviewPosition
            = tileCoord;

        LevelHandler.Instance.OngoingLevelData.Player.MoveToPosition(LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(tileCoord), reverseRotation);
    }

    private void OnUndo(PlayedCard undoneCard)
    {
        MovePlayerToTileByCoord(
            undoneCard.PlayerPosOnStartPlayCard,
            reverseRotation : true);
    }
}
