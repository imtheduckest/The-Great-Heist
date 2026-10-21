using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum CardType
{
    Valuable,
    Item,
    Event
}

[Serializable]
public class CardData
{

    public string cardName;
    public CardType cardType;
    public int value;

    public CardData(string cardName, CardType cardType, int value)
    {
        this.cardName = cardName;
        this.cardType = cardType;
        this.value = value;
    }
}
