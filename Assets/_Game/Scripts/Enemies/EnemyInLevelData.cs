using System;
using UnityEngine;

[Serializable]
public class EnemyInLevelData
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] Vector2Int spawnPosition;

    public EnemyData EnemyData => enemyData;
    public Vector2Int SpawnPosition => spawnPosition;
}
