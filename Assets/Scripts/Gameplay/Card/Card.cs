using System;

#nullable enable

[Serializable]
public class Card
{
    public RankEnum Rank;
    public SuiteEnum Suite;
    public CardView CardView = new();
}
