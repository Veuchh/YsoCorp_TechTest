using System.Collections.Generic;
using UnityEngine;

public class CardDeck : ScriptableObject
{
    [SerializeField] List<CardData> defaultUnlockedCards;

    public List<CardData> GetFullDeckCopy() 
    {
    //TODO : add unlocked card to full deck
        var fullDeck = new List<CardData>();
        fullDeck.AddRange(defaultUnlockedCards);
        return fullDeck;
    }

    public CardData DrawCardByAttribute(CardAttributes requiredAttributes)
    {
        List<CardData> fullDeckCopy = GetFullDeckCopy();
        
        while (fullDeckCopy.Count  > 0)
        {
            int i = Random.Range(0, fullDeckCopy.Count);
            
            CardData card = fullDeckCopy[i];

            //if there is a match in the flags, we allow it
            if ((card.CardAttributes & requiredAttributes) != 0)
            {
                return card;
            }

            fullDeckCopy.RemoveAt(i);
        }

        return null;
    }
}
