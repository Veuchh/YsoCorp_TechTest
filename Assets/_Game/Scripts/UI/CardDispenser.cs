using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class CardDispenser : MonoBehaviour
{
    [SerializeField] UICard uiCardPrefab;
    [SerializeField] CardDeck cardDeck;

    //This will be refilled when undoing
    Stack<CardData> undoneCards = new Stack<CardData>();
    CardAttributes availableCardsAttributes;
    UICard currentCard;
    CardSelectionService selectionService;

    public void Initialize(CardAttributes availableCardsAttributes, CardSelectionService selectionService)
    {
        this.availableCardsAttributes = availableCardsAttributes;
        this.selectionService = selectionService;

        selectionService.OnCardSelected.AddListener(HandleCardSelected);
        selectionService.OnCardDeselected.AddListener(HandleCardDeselected);

        DrawCard();
    }

    void OnDestroy()
    {
        if (selectionService == null) return;

        selectionService.OnCardSelected.RemoveListener(HandleCardSelected);
        selectionService.OnCardDeselected.RemoveListener(HandleCardDeselected);
    }

    private void DrawCard()
    {
        CardData cardData;


        if (undoneCards.Count > 0)
        {
            cardData = undoneCards.Pop();
        }
        else
        {
            cardData = cardDeck.DrawCardByAttribute(availableCardsAttributes);
            if (cardData == null)
            {
                Debug.Log($"Could not find unlocked card with the following attributes : {availableCardsAttributes} " +
                    $"when trying to draw card from {gameObject.name}", this);
                return;
            }
        }

        UnsubscribeFromCardEvent(currentCard);

        Vector3 cardSpawnPosition =
            new Vector3(-Screen.width * .75f,
            -Screen.height * .75f,
            0f);

        currentCard = Instantiate(uiCardPrefab, cardSpawnPosition, Quaternion.identity, transform);
        currentCard.PlayDrawCardTween();
        currentCard.Initialize(cardData);

        SubscribeToCardEvent(currentCard);
    }

    private void SubscribeToCardEvent(UICard card)
    {
        if (card == null)
        {
            Debug.LogError($"Trying to subscribe to the event of a non existing card.", this);
            return;
        }

        card.OnCardClicked.AddListener(OnCardClicked);
    }

    private void UnsubscribeFromCardEvent(UICard card)
    {
        if (card == null)
        {
            return;
        }
        card.OnCardClicked.RemoveListener(OnCardClicked);
    }

    private void OnCardClicked(CardData cardData)
    {
        selectionService.Select(this, cardData);
    }

    private void HandleCardSelected(CardDispenser dispenser, CardData card)
    {
        if (dispenser == this)
            currentCard.PlaySelectCardTween();
        else
            currentCard?.PlayDeselectCardTween();
    }

    private void HandleCardDeselected(CardDispenser dispenser)
    {
        if (dispenser == this)
            currentCard?.PlayDeselectCardTween();
    }

}
