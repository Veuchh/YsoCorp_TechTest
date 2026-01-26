using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2Int GridSize = new Vector2Int(4, 6);
    public Vector2Int PlayerStartPos = new Vector2Int(0,0);
}
