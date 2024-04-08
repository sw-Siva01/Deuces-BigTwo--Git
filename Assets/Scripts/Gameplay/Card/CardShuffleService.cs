using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public class CardShuffleService : Common, ICardShuffleService
{
    private List<int> _mapIndex;
    private int _firstCardIndexCache = 0;

    private readonly ICardCollection _cardCollection;


    public CardShuffleService(ICardCollection cardCollection)
    {
        _cardCollection = cardCollection;

        _mapIndex = new List<int>();
        for (int i = 0; i < _cardCollection.Cards.Count; i++)
        {
            _mapIndex.Add(i);
        }

        CacheFirstCardIndex();
    }

    private void CacheFirstCardIndex()
    {
        // TODO consider move this functionality to ICardCollection
        _firstCardIndexCache = _cardCollection.Cards
            .FindIndex(x => x.Rank == RankEnum.Three && x.Suite == SuiteEnum.Diamond);
       
        Debug.LogFormat("First turn card index: {0}", _firstCardIndexCache);
        Debug.LogFormat(
            "First turn card: {0} {1}",
            _cardCollection.Cards[_firstCardIndexCache].Rank,
            _cardCollection.Cards[_firstCardIndexCache].Suite);

        isSet = true; isON = true;
        Debug.Log(isSet);
    }

    public Dictionary<int, List<int>> Shuffle(int playerCount)
    {
        _mapIndex.Shuffle();

        int cardsPerPlayer = _cardCollection.Cards.Count / playerCount;
        Dictionary<int, List<int>> playerShuffleResults = new();
        for (int i = 0; i < playerCount; i++)
        {
            List<int> shuffleResult = new();
            shuffleResult.AddRange(_mapIndex.GetRange(i * cardsPerPlayer, cardsPerPlayer));
            playerShuffleResults.Add(i, shuffleResult);
        }

        return playerShuffleResults;
    }

    public int GetFirstTurnPlayerIndex(Dictionary<int, List<int>> shuffleResult)
    {
        foreach (var player in shuffleResult)
        {
            foreach (var cardIndex in player.Value)
            {
                if (cardIndex == _firstCardIndexCache)
                {
                    return player.Key;
                }
            }
        }

        throw new InvalidOperationException("First card index not found!");
    }
}
