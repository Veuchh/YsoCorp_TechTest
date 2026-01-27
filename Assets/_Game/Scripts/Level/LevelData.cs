using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    //The reason every field is public is to facilitate the creation of a potential LevelCreator Tool

    public Vector2Int GridSize = new Vector2Int(4, 6);
    public Vector2Int PlayerStartPos = new Vector2Int(0, 0);

    [Tooltip("Every entry in this list will be a card dispenser in the level." +
        "The chosen values determines what type of card can be drawed by the dispenser.")]
    public List<CardAttributes> cardDispenserAttributes;
}
