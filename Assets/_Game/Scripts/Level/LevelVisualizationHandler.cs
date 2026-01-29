using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelVisualizationHandler : MonoBehaviour
{
    [SerializeField] float beginVisualizationScreenOverlayDuration = 1f;
    [SerializeField] float playerTurnActionDuration = .4f;
    [SerializeField] float enemyTickActionDuration = .3f;

    private async void Awake()
    {

        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnStartVisualization.AddListener(BeginVisualization);
    }

    private void OnDestroy()
    {
        LevelHandler.Instance?.OnStartVisualization.RemoveListener(BeginVisualization);
    }

    private async void BeginVisualization()
    {
        await UniTask.Delay(Mathf.RoundToInt(beginVisualizationScreenOverlayDuration / 2 * 1000));

        List<LevelStateOnAction> allPlayedLevelStates = new List<LevelStateOnAction>(LevelHandler.Instance.OngoingLevelData.LevelStates);
        ResetLevelToBaseState(allPlayedLevelStates);

        await UniTask.Delay(Mathf.RoundToInt(beginVisualizationScreenOverlayDuration / 2 * 1000));

        ResolveStatesSequentially(allPlayedLevelStates);
    }

    private void ResetLevelToBaseState(List<LevelStateOnAction> allPlayedLevelStates)
    {
        for (int i = 0; i < allPlayedLevelStates.Count; i++)
        {
            LevelHandler.Instance.TryUndo();
        }
    }

    private async void ResolveStatesSequentially(List<LevelStateOnAction> allStates)
    {
        //we ignore the first state, since it is the base one
        for (int i = 1; i < allStates.Count; i++)
        {
            LevelStateOnAction currentState = allStates[i];
            LevelStateOnAction previousState = allStates[i - 1];
            //Player action (same time as enemy death, if any)
            await ResolvePlayerAction(currentState, previousState);

            //Enemy tick
            await ResolveEnemyTick(currentState);

            //Check for early defeat condition
            //The levelhandler.Lose method is called in this function, in order to be able to specify wich enemy caused trhe loss
            if (CheckForEarlyDefeatConditions(currentState) != DefeatReason.None)
            {
                return;
            }
        }

        // Check for victory condition (all enemies dead)
        FinalVictoryCheck(allStates.Last());
    }

    private async UniTask ResolvePlayerAction(LevelStateOnAction newState, LevelStateOnAction previousState)
    {
        Player player = LevelHandler.Instance.OngoingLevelData.Player;

        if (newState.PlayerPosOnStartPlayCard != previousState.PlayerPosOnStartPlayCard)
        {
            Vector3 newPlayerPos = LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(newState.PlayerPosOnStartPlayCard);
            player.MoveToPosition(newPlayerPos, false);
        }

        foreach (var cardEffect in previousState.CardData.CardEffects)
        {
            switch (cardEffect.EffectType)
            {
                case CardEffectType.RotateTowardsClickedTile:
                    player.RotateTowardsPosition(
                        LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(previousState.ClickedTileCoord));
                    break;
                case CardEffectType.TriggerPlayerAnimation:
                    player.TriggerAnimation(cardEffect.AnimationID);
                    break;
            }
        }

        foreach (var enemyState in newState.EnemiesState)
        {
            if (!enemyState.IsAlive)
            {
                enemyState.EnemyReference.TryKill();
            }
        }

        await UniTask.Delay(Mathf.RoundToInt(playerTurnActionDuration * 1000));
    }

    private async UniTask ResolveEnemyTick(LevelStateOnAction newState)
    {
        foreach (var enemyState in newState.EnemiesState)
        {
            if (enemyState.IsAlive)
            {
                //Normal movement
                if (enemyState.CoordBeforePlayedCard.y >= 0)
                {
                    Vector3 targetMoveCoord = LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(enemyState.CoordBeforePlayedCard);
                    enemyState.EnemyReference.PlayMoveAnim(targetMoveCoord);
                }
                //Exiting map movement
                else
                {
                    enemyState.EnemyReference.PlayReachEndOfMapAnim();
                    enemyState.EnemyReference.SetHasReachedEndOfMap(true);
                }
            }
        }

        await UniTask.Delay(Mathf.RoundToInt(enemyTickActionDuration * 1000));
    }

    private DefeatReason CheckForEarlyDefeatConditions(LevelStateOnAction levelState)
    {
        foreach (EnemyState enemyState in levelState.EnemiesState)
        {
            if (enemyState.HasEnemyReachedEndOfMap)
            {
                LevelHandler.Instance.LoseLevel(DefeatReason.EnemyReachedBottom, enemyState.EnemyReference);
                return DefeatReason.EnemyReachedBottom;
            }

            if (enemyState.IsAlive &&
                levelState.PlayerPosOnStartPlayCard == enemyState.CoordBeforePlayedCard)
            {
                LevelHandler.Instance.LoseLevel(DefeatReason.SameTileAsEnemy, enemyState.EnemyReference);
                return DefeatReason.SameTileAsEnemy;
            }
        }

        return DefeatReason.None;
    }

    private void FinalVictoryCheck(LevelStateOnAction finalState)
    {
        foreach (EnemyState enemyState in finalState.EnemiesState)
        {
            if (enemyState.IsAlive)
            {
                LevelHandler.Instance.LoseLevel(DefeatReason.EnemyAliveAfterVisualization, enemyState.EnemyReference);
                return;
            }
        }

        LevelHandler.Instance.WinLevel();
    }
}
