using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public class SubmittableCombinationService : ISubmittableCombinationService
{
    private Dictionary<RankEnum, List<Card>> _sameRankCountHashMap = new();
    private Dictionary<SuiteEnum, List<Card>> _sameSuiteCountHashMap = new();

    public void GetCombination(
        List<Card> cards,
        List<ISubmittableCard> submittables)
    {
        _sameRankCountHashMap.Clear();
        _sameSuiteCountHashMap.Clear();

        // Sort the cards by rank
        cards.Sort((x, y) => x.Rank.CompareTo(y.Rank));


        foreach (var card in cards)
        {
            // Get "Single" combination
            submittables.Add(new SubmittableCard(
                PokerHandEnum.Single,
                new List<Card>() { card }));

            if (!_sameRankCountHashMap.ContainsKey(card.Rank))
            {
                _sameRankCountHashMap.Add(card.Rank, new List<Card>() { card });
            }
            else
            {
                _sameRankCountHashMap[card.Rank].Add(card);
            }

            if (!_sameSuiteCountHashMap.ContainsKey(card.Suite))
            {
                _sameSuiteCountHashMap.Add(card.Suite, new List<Card>() { card });
            }
            else
            {
                _sameSuiteCountHashMap[card.Suite].Add(card);
            }
        }

        foreach (var sameRankCard in _sameRankCountHashMap)
        {
            foreach (var sameSuitCard in _sameSuiteCountHashMap)
            {
                if (sameRankCard.Value != null)
                {
                    // Get "Pair" combination
                    if (sameRankCard.Value.Count == 2)
                    {
                        submittables.Add(new SubmittableCard(
                            PokerHandEnum.Pair,
                            sameRankCard.Value));
                    }
                    else if (sameRankCard.Value.Count == 3) // Get "ThreeOfAKind" combination
                    {
                        submittables.Add(new SubmittableCard(
                            PokerHandEnum.ThreeOfAKind,
                            sameRankCard.Value));
                    }
                    else if (sameSuitCard.Value.Count == 5) // Get "Flush" combination
                    {
                        submittables.Add(new SubmittableCard(
                            PokerHandEnum.Flush,
                            sameSuitCard.Value));
                    }
                    else if (sameRankCard.Value.Count == 4) // Get "FourOfAKind" combination
                    {
                        foreach (var otherCard in cards)
                        {
                            if (otherCard.Rank != sameRankCard.Key)
                            {
                                var fourOfAKindCombination = new List<Card>(sameRankCard.Value);
                                fourOfAKindCombination.Add(otherCard);
                                submittables.Add(new SubmittableCard(
                                    PokerHandEnum.FourOfAKind,
                                    fourOfAKindCombination));
                                break; // Only add one combination for each FourOfAKind
                            }
                        }
                    }
                    else if (sameRankCard.Value.Count == 3) // Get "FullHouse" combination
                    {
                        foreach (var otherRankCard in _sameRankCountHashMap)
                        {
                            if (otherRankCard.Key != sameRankCard.Key && otherRankCard.Value.Count >= 2)
                            {
                                // Found a valid FullHouse combination
                                var fullHouseCombination = new List<Card>(sameRankCard.Value);
                                fullHouseCombination.AddRange(otherRankCard.Value.GetRange(0, 2)); // Add two cards of another rank
                                submittables.Add(new SubmittableCard(
                                    PokerHandEnum.FullHouse,
                                    fullHouseCombination));
                                break; // Only add one FullHouse combination for each ThreeOfAKind
                            }
                        }
                    }
                }
            }
        }

        List<Card> straightCandidate = new List<Card>();        //Get "Straight" & "StraightFlush" combination
                                                                        // Both in samecode
        foreach (var sameRankCard in _sameRankCountHashMap)
        {
            if (sameRankCard.Value.Count > 0)
            {
                straightCandidate.Add(sameRankCard.Value[0]);
            }
            else
            {
                straightCandidate.Clear();
            }

            if (straightCandidate.Count >= 5)
            {
                // Check if the cards in straightCandidate are consecutive ranks
                bool isConsecutive = true;
                for (int i = 1; i < straightCandidate.Count; i++)
                {
                    if (straightCandidate[i].Rank - straightCandidate[i - 1].Rank != 1)
                    {
                        isConsecutive = false;
                        break;
                    }
                }

                // If the cards are consecutive, add them as a Straight combination
                if (isConsecutive)
                {
                    submittables.Add(new SubmittableCard(
                        PokerHandEnum.Straight,
                        straightCandidate.GetRange(straightCandidate.Count - 5, 5)));
                }
            }
        }


        // TODO add support to "Straight" poker hand combination

        // TODO add support to "StraightFlush" poker hand combination
    }
}
