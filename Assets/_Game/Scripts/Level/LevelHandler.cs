using Unity.Cinemachine;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public static LevelHandler Instance { get; private set; }

    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Player playerPrefab;

    Player player;
    OngoingLevelData currentOngoingLevelData;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to initialize an Intance of {nameof(LevelHandler)}, but one already existed. Aborting.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
