using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitiesHandler : MonoBehaviour
{
    public static EntitiesHandler Instance;
    [SerializeField] Player playerPrefab;
    [SerializeField] float preTickDelay = .5f;
    OngoingLevelData ongGoingLevelDataReference;

    private async void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to initialize an Intance of {nameof(EntitiesHandler)}, but one already existed. Aborting.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnGridGenerated.AddListener(OnGridGenerated);
        LevelHandler.Instance.OnCardUndone.AddListener(OnUndo);
        LevelHandler.Instance.OnEnemyTick.AddListener(Tick);
    }

    private void OnDestroy()
    {
        Instance = null;
        LevelHandler.Instance?.OnGridGenerated.RemoveListener(OnGridGenerated);
        LevelHandler.Instance?.OnCardUndone.RemoveListener(OnUndo);
        LevelHandler.Instance?.OnEnemyTick.RemoveListener(Tick);
    }

    private async void Tick()
    {
        List<EnemyState> currentEnemmiesState = new List<EnemyState>();

        foreach (EnemyEntity enemy in ongGoingLevelDataReference.EnemiesInLevel)
        {
            currentEnemmiesState.Add(
                new EnemyState(
                    enemy,
                    enemy.HasReachedEndOfMap,
                    enemy.CurrentCoord,
                    enemy.IsAlive));
        }

        ongGoingLevelDataReference.PlayedCards.Last().SetEnemiesState(currentEnemmiesState);

        await UniTask.Delay(Mathf.RoundToInt(preTickDelay * 1000));

        foreach (EnemyEntity enemy in ongGoingLevelDataReference.EnemiesInLevel)
        {
            TryAdvanceEnemy(enemy);
        }
    }

    private void TryAdvanceEnemy(EnemyEntity enemy)
    {
        if (!enemy.IsAlive)
            return;

        Vector2Int targetMovementCoord = enemy.CurrentCoord;
        targetMovementCoord.y -= enemy.Data.MovementPerAction;
        enemy.SetCoordinates(targetMovementCoord);


        //Normal movement
        if (enemy.CurrentCoord.y >= 0)
        {
            Vector3 targetMoveCoord = ongGoingLevelDataReference.GetTileWorldCoordinate(enemy.CurrentCoord);
            enemy.PlayMoveAnim(targetMoveCoord);
        }
        //Exiting map movement
        else
        {
            enemy.PlayReachEndOfMapAnim();
            enemy.SetHasReachedEndOfMap(true);
        }
    }

    private void OnGridGenerated()
    {
        TryClearEntities();
        ongGoingLevelDataReference = LevelHandler.Instance.OngoingLevelData;

        SpawnPlayer();

        foreach (EnemyInLevelData enemyInLevelData in ongGoingLevelDataReference.LevelData.EnemiesInLevel)
        {
            SpawnEnemy(enemyInLevelData);
        }
    }

    private void TryClearEntities()
    {
        if (ongGoingLevelDataReference == null)
            return;

        Destroy(ongGoingLevelDataReference.Player.gameObject);

        foreach (var enemy in ongGoingLevelDataReference.EnemiesInLevel)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void SpawnPlayer()
    {
        ongGoingLevelDataReference.Player =
            Instantiate(playerPrefab, transform);

        MovePlayerToTileByCoord(ongGoingLevelDataReference.LevelData.PlayerStartPos);
    }

    private void SpawnEnemy(EnemyInLevelData enemyInLevelData)
    {
        EnemyEntity newEnemy = Instantiate(enemyInLevelData.EnemyData.EnemyPrefab);
        Vector3 targetSpawnPos = ongGoingLevelDataReference.GetTileWorldCoordinate(enemyInLevelData.SpawnPosition);
        newEnemy.PlaySpawnAnim(targetSpawnPos);
        newEnemy.SetCoordinates(enemyInLevelData.SpawnPosition);
        newEnemy.SetData(enemyInLevelData.EnemyData);
        ongGoingLevelDataReference.EnemiesInLevel.Add(newEnemy);
    }

    public void MovePlayerToTile(Tile tile)
    {
        MovePlayerToTileByCoord(ongGoingLevelDataReference.GetTileGridCoord(tile));
    }

    public void MovePlayerToTileByCoord(Vector2Int tileCoord, bool reverseRotation = false)
    {
        ongGoingLevelDataReference.CurrentPlayerPreviewPosition
            = tileCoord;

        ongGoingLevelDataReference.Player.MoveToPosition(ongGoingLevelDataReference.GetTileWorldCoordinate(tileCoord), reverseRotation);
    }

    private void OnUndo(LevelStateOnAction undoneCard)
    {
        if (undoneCard.PlayerPosOnStartPlayCard != ongGoingLevelDataReference.CurrentPlayerPreviewPosition)
        {
            MovePlayerToTileByCoord(
                undoneCard.PlayerPosOnStartPlayCard,
                reverseRotation: true);
        }

        foreach (EnemyState enemyState in undoneCard.EnemiesState)
        {
            if (!enemyState.HasEnemyReachedEndOfMap && enemyState.IsAlive)
            {
                Vector3 targetMoveCoord = ongGoingLevelDataReference.GetTileWorldCoordinate(enemyState.CoordBeforePlayedCard);
                enemyState.EnemyReference.PlayMoveAnim(targetMoveCoord);
            }

            enemyState.EnemyReference.SetIsAlive(enemyState.IsAlive);
            enemyState.EnemyReference.SetCoordinates(enemyState.CoordBeforePlayedCard);
            enemyState.EnemyReference.SetHasReachedEndOfMap(enemyState.HasEnemyReachedEndOfMap);
        }
    }
}
