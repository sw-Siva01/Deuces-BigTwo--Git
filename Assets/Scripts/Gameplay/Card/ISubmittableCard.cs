using System.Collections.Generic;

#nullable enable

public interface ISubmittableCard
{
    PokerHandEnum PokerHand { get; }
    List<Card> Cards { get; }
}
