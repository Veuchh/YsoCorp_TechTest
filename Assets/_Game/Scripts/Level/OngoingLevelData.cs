using System;
using System.Collections.Generic;
using UnityEngine;

public class OngoingLevelData
{
    public LevelData LevelData;
    public Tile[,] Tiles;
    public Vector2Int CurrentPlayerPreviewPosition;
    public CardData CurrentlySelectedCard;
    public Tile CurrentlyHoveredTile;
    public bool IsHighlightDirty = false;
    public Player Player;

    public OngoingLevelData(LevelData levelData, Tile[,] tiles, Vector2Int playerPosition)
    {
        Tiles = tiles;
        CurrentPlayerPreviewPosition = playerPosition;
        LevelData = levelData;
    }

    public Vector3 GetTileWorldCoordinate(Tile tile)
    {
        return GetTileWorldCoordinate(GetTileGridCoord(tile));
    }

    public Vector3 GetTileWorldCoordinate(Vector2Int tileCoord)
    {
        return Tiles[tileCoord.x, tileCoord.y].transform.position;
    }

    public Vector2Int GetTileGridCoord(Tile clickedTile)
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                if (Tiles[x,y] == clickedTile)
                {
                    return new Vector2Int(x,y);
                }
            }
        }

        Debug.LogError("The tile was not found in the grid");

        return new Vector2Int(-1,-1);
    }
}
