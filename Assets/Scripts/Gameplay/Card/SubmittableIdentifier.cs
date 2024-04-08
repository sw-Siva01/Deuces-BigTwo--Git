using System.Collections.Generic;

#nullable enable

public class SubmittableIdentifier : ISubmittableIdentifier
{
    public bool TryGetSubmittable(
        List<Card> cards,
        out ISubmittableCard? submittableCard)
    {
        switch (cards.Count)
        {
            case 1:
                submittableCard = new SubmittableCard(
                    PokerHandEnum.Single,
                    new List<Card>() { cards[0] });
                return true;
            case 2:
                if (cards[0].Rank == cards[1].Rank)
                {
                    submittableCard = new SubmittableCard(
                        PokerHandEnum.Pair,
                        new List<Card>() { cards[0], cards[1] });
                    return true;
                }
                else
                {
                    submittableCard = null;
                    return false;
                }
            case 3:
                if (cards[0].Rank == cards[1].Rank && cards[0].Rank == cards[2].Rank)
                {
                    submittableCard = new SubmittableCard(
                        PokerHandEnum.ThreeOfAKind,
                        new List<Card>() { cards[0], cards[1], cards[2] });
                    return true;
                }
                else
                {
                    submittableCard = null;
                    return false;
                }
            case 5:
                // TODO support 5 poker hand
                // Sort the cards by rank
                cards.Sort((x, y) => x.Rank.CompareTo(y.Rank));

                // Check for a straight
                bool isStraight = cards[4].Rank - cards[0].Rank == 4 &&
                                  cards[0].Rank != cards[1].Rank &&
                                  cards[1].Rank != cards[2].Rank &&
                                  cards[2].Rank != cards[3].Rank &&
                                  cards[3].Rank != cards[4].Rank;
                if (isStraight)
                {
                    submittableCard = new SubmittableCard(
                        PokerHandEnum.Straight,
                        new List<Card>(cards));
                    return true;
                }
                else if (cards[0].Suite == cards[1].Suite && cards[0].Suite == cards[2].Suite && cards[0].Suite == cards[3].Suite && cards[0].Suite == cards[4].Suite)
                {
                    submittableCard = new SubmittableCard(
                        PokerHandEnum.Flush,
                        new List<Card>() { cards[0], cards[1], cards[2], cards[3], cards[4] });
                    return true;
                }
                else if (cards[0].Rank == cards[1].Rank && cards[0].Rank == cards[2].Rank && cards[0].Rank == cards[3].Rank && cards[0].Rank != cards[4].Rank)
                {
                    submittableCard = new SubmittableCard(
                        PokerHandEnum.FourOfAKind,
                        new List<Card>() { cards[0], cards[1], cards[2], cards[3], cards[4] });
                    return true;
                }
                else if ((cards[0].Rank == cards[1].Rank && cards[0].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank) ||
                            (cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[3].Rank && cards[2].Rank == cards[4].Rank))
                {
                    submittableCard = new SubmittableCard(
                         PokerHandEnum.FullHouse,
                         new List<Card>() { cards[0], cards[1], cards[2], cards[3], cards[4] });
                    return true;
                }
                else
                {
                    submittableCard = null;
                    return false;
                }
            default:
                submittableCard = null;
                return false;
        }
    }
}
