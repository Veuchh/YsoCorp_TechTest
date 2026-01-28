using System;
using UnityEngine.Events;

public class CardSelectionService
{
    public CardData SelectedCard { get; private set; }
    public CardDispenser SelectedDispenser { get; private set; }

    public UnityEvent<CardDispenser, CardData> OnCardSelected = new UnityEvent<CardDispenser, CardData>();
    public UnityEvent<CardDispenser> OnCardDeselected = new UnityEvent<CardDispenser>();
    public UnityEvent<CardDispenser> OnSelectedCardPlayed = new UnityEvent<CardDispenser>();

    public void Select(CardDispenser dispenser, CardData card)
    {
        //This means we clicked on the already selected card
        if (SelectedDispenser == dispenser)
        {
            Clear();
            return;
        }

        if (SelectedDispenser != null)
            OnCardDeselected?.Invoke(SelectedDispenser);

        SelectedDispenser = dispenser;
        SelectedCard = card;

        OnCardSelected?.Invoke(dispenser, card);
    }

    public void Clear()
    {
        if (SelectedDispenser != null)
            OnCardDeselected?.Invoke(SelectedDispenser);

        SelectedDispenser = null;
        SelectedCard = null;
    }

    public void PlaySelectedCard()
    {
        OnSelectedCardPlayed?.Invoke(SelectedDispenser);
    }
}
