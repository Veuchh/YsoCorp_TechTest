using UnityEngine;

public class StubStartLevel : MonoBehaviour
{
    [SerializeField] LevelData stubLevelData;

    void Start()
    {
        StubGenerateLevel();
    }

    [ContextMenu(nameof(StubGenerateLevel))]
    private void StubGenerateLevel()
    {
        LevelHandler.Instance.StartLevel(stubLevelData);
    }
}
