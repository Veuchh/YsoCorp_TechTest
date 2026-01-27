using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardEffectPreviewer : MonoBehaviour
{
    [SerializeField] Texture neutralTileTexture;
    [SerializeField] Texture movableTileTexture;
    [SerializeField] Texture unmovableTileTexture;
    [SerializeField] Texture hoveredTileTexture;

    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnNewCardSelected.AddListener(UpdateHighlights);
    }

    private void OnDestroy()
    {
        LevelHandler.Instance.OnNewCardSelected.RemoveListener(UpdateHighlights);
    }

    private void Update()
    {
        if (LevelHandler.Instance.OngoingLevelData != null
            && LevelHandler.Instance.OngoingLevelData.IsHighlightDirty)
        {
            UpdateHighlights();
        }
    }

    void UpdateHighlights()
    {
        Tile[,] tiles = LevelHandler.Instance.OngoingLevelData.Tiles;

        if (LevelHandler.Instance.OngoingLevelData.SelectedCard == null)
        {
            RemoveAllHighlight(tiles);
        }
        else
        {
            HighlightBasedOnCard(tiles);
        }

        LevelHandler.Instance.OngoingLevelData.IsHighlightDirty = false;
    }

    private void RemoveAllHighlight(Tile[,] tiles)
    {
        for (int x = 0; x < LevelHandler.Instance.OngoingLevelData.LevelData.GridSize.x; x++)
        {
            for (int y = 0; y < LevelHandler.Instance.OngoingLevelData.LevelData.GridSize.y; y++)
            {
                tiles[x, y].SetTexture(neutralTileTexture);
            }
        }
    }

    private void HighlightBasedOnCard(Tile[,] tiles)
    {
        CardData selectedCard = LevelHandler.Instance.OngoingLevelData.SelectedCard;

        CardEffect choseMovementCardEffect = selectedCard.GetEffectByType(CardEffectType.ChoseMovePosition);

        for (int x = 0; x < LevelHandler.Instance.OngoingLevelData.LevelData.GridSize.x; x++)
        {
            for (int y = 0; y < LevelHandler.Instance.OngoingLevelData.LevelData.GridSize.y; y++)
            {
                if (choseMovementCardEffect != null)
                {
                    Vector2Int range = choseMovementCardEffect.ChoseMoveRange;
                    Vector2Int playerPosition = LevelHandler.Instance.OngoingLevelData.CurrentPlayerPreviewPosition;
                    int positionDifference = Mathf.Abs(playerPosition.x - x) + Mathf.Abs(playerPosition.y - y);

                    if (positionDifference >= range.x
                        && positionDifference <= range.y
                        && positionDifference != 0)
                    {
                        if (LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile == tiles[x, y])
                        {
                            tiles[x, y].SetTexture(hoveredTileTexture);
                        }
                        else
                        {
                            tiles[x, y].SetTexture(movableTileTexture);
                        }
                    }
                }
                //TODO : set to unwalkable if enemy will be there next turn
                else if (false)
                {

                }
                else
                {
                    tiles[x, y].SetTexture(neutralTileTexture);
                }
            }
        }
    }
}
