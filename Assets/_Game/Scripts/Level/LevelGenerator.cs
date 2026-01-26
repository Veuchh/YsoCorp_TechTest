using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Transform spawnedTilesParent;
    [SerializeField] Tile tilePrefab;
    [SerializeField] float tileOffset = 1.5f;

    public OngoingLevelData GenerateLevel(LevelData levelData)
    {
        TryClearPreExistingLevel();

        Tile[,] instantiatedTiles = new Tile[levelData.GridSize.x, levelData.GridSize.y];


        for (int y = 0; y < levelData.GridSize.y; y++)
        {
            for (int x = 0; x < levelData.GridSize.x; x++)
            {
                Tile newTile = Instantiate(tilePrefab, spawnedTilesParent);

                Vector3 newTileLocalPos = new Vector3(
                    (x - (levelData.GridSize.x - 1) / 2f) * tileOffset,
                    0,
                    tileOffset * y);

                newTile.transform.localPosition = newTileLocalPos;

                instantiatedTiles[x, y] = newTile;
            }
        }


        return new OngoingLevelData(
            levelData: levelData,
            tiles : instantiatedTiles,
            playerPosition : levelData.PlayerStartPos);
    }

    private void TryClearPreExistingLevel()
    {
        for (int i = spawnedTilesParent.childCount -1; i >= 0; i--)
        {
            Destroy(spawnedTilesParent.GetChild(i).gameObject);
        }
    }
}
