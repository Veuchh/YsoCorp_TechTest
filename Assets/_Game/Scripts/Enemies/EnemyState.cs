using UnityEngine;

public class EnemyState
{
    EnemyEntity enemyReference;
    bool hasEnemyReachedEndOfMap;
    Vector2Int coordBeforePlayedCard;
    bool isAlive;

    public EnemyEntity EnemyReference => enemyReference;
    public bool HasEnemyReachedEndOfMap => hasEnemyReachedEndOfMap;
    public Vector2Int CoordBeforePlayedCard => coordBeforePlayedCard;
    public bool IsAlive => isAlive;

    public EnemyState(EnemyEntity enemyReference, bool hasEnemyReachedEndOfMap, Vector2Int coordBeforePlayedCard, bool isAlive)
    {
        this.enemyReference = enemyReference;
        this.hasEnemyReachedEndOfMap = hasEnemyReachedEndOfMap;
        this.coordBeforePlayedCard = coordBeforePlayedCard;
        this.isAlive = isAlive;
    }
}
