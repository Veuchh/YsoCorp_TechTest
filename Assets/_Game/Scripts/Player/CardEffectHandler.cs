using Cysharp.Threading.Tasks;
using System;
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
        LevelHandler.Instance.OnTileClicked.RemoveListener(OnTileClicked);
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
        foreach (CardEffect cardEffect in currentlySelectedCard.CardEffects)
        {
            switch (cardEffect.EffectType)
            {
                case CardEffectType.MoveToClickedTile:
                    MovePlayerToPosition(clickedTileCoord);
                    break;
            }
        }

        LevelHandler.Instance.PlayCardOnTile(currentlySelectedCard, clickedTileCoord);

        LevelHandler.Instance.OngoingLevelData.CurrentlySelectedCard = null;
        LevelHandler.Instance.OngoingLevelData.IsHighlightDirty = true;
        //TODO tick entity handler
    }

    private void MovePlayerToPosition(Vector2Int newPosition)
    {
        //TODO call entityHandler move player
        LevelHandler.Instance.OngoingLevelData.CurrentPlayerPreviewPosition = newPosition;
        EntitiesHandler.Instance.MovePlayerToTileByCoord(newPosition);
    }
}
