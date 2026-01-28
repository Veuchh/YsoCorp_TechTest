using UnityEngine;

public class EnemyState
{
    EnemyEntity enemyReference;
    bool hasEnemyReachedEndOfMap;
    Vector2Int coordBeforePlayedCard;

    public EnemyEntity EnemyReference => enemyReference;
    public bool HasEnemyReachedEndOfMap => hasEnemyReachedEndOfMap;
    public Vector2Int CoordBeforePlayedCard => coordBeforePlayedCard;

    public EnemyState(EnemyEntity enemyReference, bool hasEnemyReachedEndOfMap, Vector2Int coordBeforePlayedCard)
    {
        this.enemyReference = enemyReference;
        this.hasEnemyReachedEndOfMap = hasEnemyReachedEndOfMap;
        this.coordBeforePlayedCard = coordBeforePlayedCard;
    }
}
