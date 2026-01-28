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
        LevelHandler.Instance?.OnNewCardSelected.RemoveListener(UpdateHighlights);
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

        if (LevelHandler.Instance.OngoingLevelData.CurrentlySelectedCard == null)
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
        CardData selectedCard = LevelHandler.Instance.OngoingLevelData.CurrentlySelectedCard;

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                TileValidity tileValdity = CardValidityCheck.CheckTileValidityForCard(tiles[x, y], selectedCard);

                HighlightTileBasedOnValidity(tiles[x, y], tileValdity);
            }
        }
    }

    private void HighlightTileBasedOnValidity(Tile tile, TileValidity validity) 
    {
        switch (validity)
        {
            case TileValidity.Neutral:
                tile.SetTexture(neutralTileTexture);
                break;

            case TileValidity.Valid:
                if (LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile == tile)
                {
                    tile.SetTexture(hoveredTileTexture);
                }
                else
                {
                    tile.SetTexture(movableTileTexture);
                }
                break;

            case TileValidity.Invalid:
                tile.SetTexture(unmovableTileTexture);
                break;
        }
    }
}