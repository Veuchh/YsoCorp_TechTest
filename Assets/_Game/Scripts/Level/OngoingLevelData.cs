using System.Collections.Generic;
using UnityEngine;

public class OngoingLevelData
{
    public LevelData LevelData;
    public Tile[,] Tiles;
    public Vector2Int CurrentPlayerPreviewPosition;
    public CardData SelectedCard;
    public Tile CurrentlyHoveredTile;
    public bool IsHighlightDirty = false;

    public OngoingLevelData(LevelData levelData, Tile[,] tiles, Vector2Int playerPosition)
    {
        Tiles = tiles;
        CurrentPlayerPreviewPosition = playerPosition;
        LevelData = levelData;
    }


    public Vector3 GetTileWorldCoordinate(Vector2Int tileCoord)
    {
        return Tiles[tileCoord.x, tileCoord.y].transform.position;
    }
}
