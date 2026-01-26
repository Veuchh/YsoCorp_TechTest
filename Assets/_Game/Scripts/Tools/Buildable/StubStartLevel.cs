using UnityEngine;

public class StubStartLevel : MonoBehaviour
{
    [SerializeField] LevelData stubLevelData;
    [SerializeField] LevelHandler levelHandler;

    void Start()
    {
        StubGenerateLevel();
    }

    [ContextMenu(nameof(StubGenerateLevel))]
    private void StubGenerateLevel()
    {
        levelHandler.StartLevel(stubLevelData);
    }
}
