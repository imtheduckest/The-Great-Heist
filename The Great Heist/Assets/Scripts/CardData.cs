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

    [TextArea(3, 6)]
    public string description;

    public CardDefinition cardDefinition;

    public CardData(
        string cardName,
        CardType cardType,
        int value,
        string description = "",
        CardDefinition cardDefinition = null)
    {
        this.cardName = cardName;
        this.cardType = cardType;
        this.value = value;
        this.description = description;
        this.cardDefinition = cardDefinition;
    }
}
