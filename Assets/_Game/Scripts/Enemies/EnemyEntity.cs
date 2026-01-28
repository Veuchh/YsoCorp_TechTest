using DG.Tweening;
using System;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    const string ANIMATOR_DEATH_KEY = "isDead";

    [Header("Tween - Spawn")]
    [SerializeField] float spawnAnimDuration = .6f;
    [SerializeField] float spawnAnimOffset = 2f;

    [Header("Tween - Move")]
    [SerializeField] float moveAnimDuration = .35f;

    [Header("Tween - Reach end of Map")]
    [SerializeField] float endAnimDuration = .35f;
    [SerializeField] float endAnimOffset = 2f;

    Sequence currentTween;
    Animator animator;

    EnemyData data;
    Vector2Int currentCoord;
    bool hasReachedEndOfMap = false;
    bool isAlive = true;

    public EnemyData Data => data;
    public Vector2Int CurrentCoord => currentCoord;
    public bool HasReachedEndOfMap => hasReachedEndOfMap;
    public bool IsAlive => isAlive;


    public void SetData(EnemyData data) => this.data = data;
    public void SetCoordinates(Vector2Int newCoord) => currentCoord = newCoord;
    public void SetHasReachedEndOfMap(bool hasReached) => hasReachedEndOfMap = hasReached;
    public void SetIsAlive(bool isAlive) => this.isAlive = isAlive;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

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

        //if it is moving, then it is alove, so we can safely set this
        animator.SetBool(ANIMATOR_DEATH_KEY, false);
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

    public void TryKill()
    {
        if (!isAlive)
            return;

        isAlive = false;

        animator.SetBool(ANIMATOR_DEATH_KEY, true);
    }
}
