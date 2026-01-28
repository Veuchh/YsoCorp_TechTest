using UnityEngine;

public class PlayedCard
{
    CardData cardData;
    Vector2Int playerPosOnStartPlayCard;
    Vector2Int clickedTileCoord;
    CardDispenser originDispenser;

    public CardData CardData => cardData;
    public Vector2Int PlayerPosOnStartPlayCard => playerPosOnStartPlayCard;
    public Vector2Int ClickedTileCoord => clickedTileCoord;
    public CardDispenser OriginDispenser => originDispenser;

    public PlayedCard(CardData cardData, Vector2Int playerPosOnStartPlayCard, Vector2Int clickedTileCoord, CardDispenser originDispenser)
    {
        this.cardData = cardData;
        this.playerPosOnStartPlayCard = playerPosOnStartPlayCard;
        this.clickedTileCoord = clickedTileCoord;
        this.originDispenser = originDispenser;
    }
}
