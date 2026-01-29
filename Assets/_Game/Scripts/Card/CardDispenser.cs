using System.Collections.Generic;
using UnityEngine;

public class CardDispenser : MonoBehaviour
{
    [SerializeField] UICard uiCardPrefab;
    [SerializeField] CardDeck cardDeck;
    [SerializeField] GameObject attackAttributeLabel;
    [SerializeField] GameObject moveAttributeLabel;
    [SerializeField] GameObject waitAttributeLabel;
    [SerializeField] AudioData drawCardAudio;

    Stack<CardData> knownBelowCards = new Stack<CardData>();
    CardAttributes availableCardsAttributes;
    UICard currentCard;
    CardDrawAndSelectService selectionService;

    public void Initialize(CardAttributes availableCardsAttributes, CardDrawAndSelectService selectionService)
    {
        this.availableCardsAttributes = availableCardsAttributes;
        this.selectionService = selectionService;

        selectionService.OnCardSelected.AddListener(HandleCardSelected);
        selectionService.OnCardDeselected.AddListener(HandleCardDeselected);
        selectionService.OnSelectedCardPlayed.AddListener(TryPlayCard);
        selectionService.OnUndo.AddListener(Undo);

        //Display attack attribute
        if (availableCardsAttributes.HasFlag(CardAttributes.Tutorial_Attack)
            || availableCardsAttributes.HasFlag(CardAttributes.Attack)
            || availableCardsAttributes.HasFlag(CardAttributes.LongAttack))
        {
            attackAttributeLabel.SetActive(true);
        }
        
        //Display move attribute
        if (availableCardsAttributes.HasFlag(CardAttributes.Tutorial_Move)
            || availableCardsAttributes.HasFlag(CardAttributes.Movement)
            || availableCardsAttributes.HasFlag(CardAttributes.LongMovement))
        {
            moveAttributeLabel.SetActive(true);
        }

        //Display wait attribute
        if (availableCardsAttributes.HasFlag(CardAttributes.Wait))
        {
            waitAttributeLabel.SetActive(true);
        }

        DrawRandomCard();
    }

    void OnDestroy()
    {
        if (selectionService == null) return;

        selectionService.OnCardSelected.RemoveListener(HandleCardSelected);
        selectionService.OnCardDeselected.RemoveListener(HandleCardDeselected);
        selectionService.OnSelectedCardPlayed.RemoveListener(TryPlayCard);
        selectionService.OnUndo.RemoveListener(Undo);
    }

    private void DrawRandomCard()
    {
        CardData cardData = cardDeck.DrawCardByAttribute(availableCardsAttributes);

        if (knownBelowCards.Count != 0)
        {
            cardData = knownBelowCards.Pop();
        }

        if (cardData == null)
        {
            Debug.Log($"Could not find unlocked card with the following attributes : {availableCardsAttributes} " +
                $"when trying to draw card from {gameObject.name}", this);
            return;
        }

        DrawCard(cardData);
    }

    private void DrawCard(CardData card)
    {
        if (currentCard != null)
        {
            Destroy(currentCard.gameObject);
            UnsubscribeFromCardEvent(currentCard);
        }

        if (card == null)
        {
            Debug.LogError("CardData should not be null when drawing a card", this);
            return;
        }

        Vector3 cardSpawnPosition =
            new Vector3(-Screen.width * .75f,
            -Screen.height * .75f,
            0f);

        currentCard = Instantiate(uiCardPrefab, cardSpawnPosition, Quaternion.identity, transform);
        currentCard.PlayDrawCardTween();
        currentCard.Initialize(card);

        AudioManager.Instance.PlayAudioData(drawCardAudio);

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
        if (dispenser != this)
            return;

        currentCard?.PlayDeselectCardTween();
    }

    private void TryPlayCard(CardDispenser dispenser)
    {
        if (dispenser != this)
            return;

        currentCard.PlayPlayedCardTween();

        DrawRandomCard();
    }

    private void Undo(LevelStateOnAction playedCard)
    {
        if (playedCard.OriginDispenser != this)
            return;

        knownBelowCards.Push(currentCard.CardData);
        DrawCard(playedCard.CardData);
    }
}
