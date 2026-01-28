using System;
using UnityEngine.Events;

public class CardDrawAndSelectService
{
    public CardDispenser SelectedDispenser { get; private set; }

    public UnityEvent<CardDispenser, CardData> OnCardSelected = new UnityEvent<CardDispenser, CardData>();
    public UnityEvent<CardDispenser> OnCardDeselected = new UnityEvent<CardDispenser>();
    public UnityEvent<CardDispenser> OnSelectedCardPlayed = new UnityEvent<CardDispenser>();
    public UnityEvent<PlayedCard> OnUndo = new UnityEvent<PlayedCard>();

    public void Select(CardDispenser dispenser, CardData card)
    {
        //This means we clicked on the already selected card
        if (SelectedDispenser == dispenser)
        {
            ClearSelectedCard();
            return;
        }

        if (SelectedDispenser != null)
            OnCardDeselected?.Invoke(SelectedDispenser);

        SelectedDispenser = dispenser;

        OnCardSelected?.Invoke(dispenser, card);
    }

    public void ClearSelectedCard()
    {
        if (SelectedDispenser != null)
            OnCardDeselected?.Invoke(SelectedDispenser);

        SelectedDispenser = null;
    }

    public void PlaySelectedCard()
    {
        OnSelectedCardPlayed?.Invoke(SelectedDispenser);
        SelectedDispenser = null;
    }

    public void Undo(PlayedCard playedCard)
    {
        ClearSelectedCard();
        OnUndo?.Invoke(playedCard);
    }
}
