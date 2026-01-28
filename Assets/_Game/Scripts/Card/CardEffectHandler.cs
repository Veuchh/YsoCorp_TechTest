using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnTileClicked.AddListener(OnTileClicked);
    }

    private void OnDestroy()
    {
        LevelHandler.Instance?.OnTileClicked.RemoveListener(OnTileClicked);
    }

    private void OnTileClicked(Tile clickedTile)
    {
        CardData currentlySelectedCard = LevelHandler.Instance.OngoingLevelData.CurrentlySelectedCard;

        if (currentlySelectedCard == null)
        {
            //TODO : maybe add feedback
            return;
        }

        Vector2Int clickedTileCoord = LevelHandler.Instance.OngoingLevelData.GetTileGridCoord(clickedTile);

        TileValidity tileValidity = CardValidityCheck.CheckTileValidityForCard(clickedTile, currentlySelectedCard);

        if (tileValidity != TileValidity.Valid)
        {
            //TODO : maybe add feedback
            return;
        }

        PlayCard(currentlySelectedCard, clickedTileCoord);
    }

    private void PlayCard(CardData currentlySelectedCard, Vector2Int clickedTileCoord)
    {
        LevelHandler.Instance.PlayCardOnTile(currentlySelectedCard, clickedTileCoord);

        foreach (CardEffect cardEffect in currentlySelectedCard.CardEffects)
        {
            switch (cardEffect.EffectType)
            {
                case CardEffectType.MoveToClickedTile:
                    MovePlayerToPosition(clickedTileCoord);
                    break;
                case CardEffectType.AttackClickedTile:
                    AttackTile(clickedTileCoord);
                    break;
                case CardEffectType.RotateTowardsClickedTile:
                    LevelHandler.Instance.OngoingLevelData.Player.RotateTowardsPosition(
                        LevelHandler.Instance.OngoingLevelData.GetTileWorldCoordinate(clickedTileCoord));
                    break;
                case CardEffectType.TriggerPlayerAnimation:
                    LevelHandler.Instance.OngoingLevelData.Player.TriggerAnimation(cardEffect.AnimationID);
                    break;
            }
        }

        LevelHandler.Instance.OngoingLevelData.CurrentlySelectedCard = null;
        LevelHandler.Instance.OngoingLevelData.IsHighlightDirty = true;
    }

    private void MovePlayerToPosition(Vector2Int newPosition)
    {
        LevelHandler.Instance.OngoingLevelData.CurrentPlayerPreviewPosition = newPosition;
        EntitiesHandler.Instance.MovePlayerToTileByCoord(newPosition);
    }

    private void AttackTile(Vector2Int attackedCoord)
    {
        List<EnemyEntity> enemies = LevelHandler.Instance.OngoingLevelData.EnemiesInLevel;

        foreach (var enemy in enemies)
        {
            if (enemy.CurrentCoord == attackedCoord)
            {
                enemy.TryKill();
            }
        }
    }
}
