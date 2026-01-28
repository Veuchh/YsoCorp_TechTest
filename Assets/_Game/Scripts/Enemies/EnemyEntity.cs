using DG.Tweening;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    [Header("Tween - Spawn")]
    [SerializeField] float spawnAnimDuration = .6f;
    [SerializeField] float spawnAnimOffset = 2f;

    [Header("Tween - Move")]
    [SerializeField] float moveAnimDuration = .35f;

    [Header("Tween - Reach end of Map")]
    [SerializeField] float endAnimDuration = .35f;
    [SerializeField] float endAnimOffset = 2f;

    Sequence currentTween;

    EnemyData data;
    Vector2Int currentCoord;
    bool hasReachedEndOfMap = false;

    public EnemyData Data => data;
    public Vector2Int CurrentCoord => currentCoord;
    public bool HasReachedEndOfMap => hasReachedEndOfMap;


    public void SetData(EnemyData data) => this.data = data;
    public void SetCoordinates(Vector2Int newCoord) => currentCoord = newCoord;
    public void SetHasReachedEndOfMap(bool hasReached) => hasReachedEndOfMap = hasReached;

    public void PlaySpawnAnim(Vector3 targetPosition)
    {
        TryKillCurrentTween();

        transform.position = targetPosition + Vector3.down * spawnAnimOffset;
        
        currentTween = DOTween.Sequence();
        
        currentTween.Append(transform.DOMove(targetPosition, spawnAnimDuration));
    }

    public void PlayMoveAnim(Vector3 targetPosition)
    {
        TryKillCurrentTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(transform.DOMove(targetPosition, moveAnimDuration));
    }

    public void PlayReachEndOfMapAnim()
    {
        TryKillCurrentTween();

        currentTween = DOTween.Sequence();

        Vector2Int bottomCoord = new Vector2Int(currentCoord.x, 0);

        Vector3 targetPosition = LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(bottomCoord) + Vector3.back * endAnimOffset;

        currentTween.Append(transform.DOMove(targetPosition, endAnimDuration));
    }

    void TryKillCurrentTween()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }
    }
}
