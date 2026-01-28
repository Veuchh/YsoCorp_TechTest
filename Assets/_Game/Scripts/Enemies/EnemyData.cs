using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] EnemyEntity enemyPrefab;
    [SerializeField] int movementPerAction;

    public EnemyEntity EnemyPrefab => enemyPrefab;
    public int MovementPerAction => movementPerAction;
}
