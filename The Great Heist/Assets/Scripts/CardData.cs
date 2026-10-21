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

    [TextArea]
    public string description;

    public CardData(
        string cardName,
        CardType cardType,
        int value,
        string description = "")
    {
        this.cardName = cardName;
        this.cardType = cardType;
        this.value = value;
        this.description = description;
    }
}
