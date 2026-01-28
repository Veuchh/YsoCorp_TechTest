using UnityEngine;

public static class CardValidityCheck
{
    public static TileValidity CheckTileValidityForCard(Tile tile, CardData card)
    {
        switch (card.ValidTileCheckType)
        {
            case ValidTileCheckType.AnyInCircularRange:
                return CheckValidityForAnyInCircularRange(tile, card);
        }
        return TileValidity.Neutral;
    }

    private static TileValidity CheckValidityForAnyInCircularRange(Tile tile, CardData card)
    {
        Vector2Int playerPosition = LevelHandler.Instance.OngoingLevelData.CurrentPlayerPreviewPosition;
        Vector2Int tileCoord = LevelHandler.Instance.OngoingLevelData.GetTileGridCoord(tile);

        Vector2Int range = card.CircularRange;
        int positionDifference = Mathf.Abs(playerPosition.x - tileCoord.x) + Mathf.Abs(playerPosition.y - tileCoord.y);

        if (positionDifference >= range.x
            && positionDifference <= range.y)
        {
            return TileValidity.Valid;
        }

        //TODO : change if enemy will be there

        return TileValidity.Neutral;
    }
}