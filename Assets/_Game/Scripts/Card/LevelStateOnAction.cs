using System.Collections.Generic;
using UnityEngine;

public class LevelStateOnAction
{
    CardData cardData;
    Vector2Int playerPosOnStartPlayCard;
    Vector2Int clickedTileCoord;
    CardDispenser originDispenser;
    List<EnemyState> enemiesState;

    public CardData CardData => cardData;
    public Vector2Int PlayerPosOnStartPlayCard => playerPosOnStartPlayCard;
    public Vector2Int ClickedTileCoord => clickedTileCoord;
    public CardDispenser OriginDispenser => originDispenser;
    public List<EnemyState> EnemiesState => enemiesState;

    public LevelStateOnAction(CardData cardData, Vector2Int playerPosOnStartPlayCard, Vector2Int clickedTileCoord, CardDispenser originDispenser)
    {
        this.cardData = cardData;
        this.playerPosOnStartPlayCard = playerPosOnStartPlayCard;
        this.clickedTileCoord = clickedTileCoord;
        this.originDispenser = originDispenser;
    }

    public void SetEnemiesState(List<EnemyState> enemiesState)
    {
        this.enemiesState = enemiesState;
    }
}
