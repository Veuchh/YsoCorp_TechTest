using Unity.Cinemachine;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Player playerPrefab;

    Player player;
    OngoingLevelData currentOngoingLevelData;

    public void StartLevel(LevelData data)
    {
        ClearLevel();

        currentOngoingLevelData = levelGenerator.GenerateLevel(data);

        cameraManager.SetCameraPosition(currentOngoingLevelData.Tiles);

        player = Instantiate(playerPrefab);
        player.transform.position = currentOngoingLevelData.GetTileWorldCoordinate(currentOngoingLevelData.LevelData.PlayerStartPos);
    }

    void ClearLevel()
    {
        if (player)
        {
            Destroy(player.gameObject);
        }
    }
}
