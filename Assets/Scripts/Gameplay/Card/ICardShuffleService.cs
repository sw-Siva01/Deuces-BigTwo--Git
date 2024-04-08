using System.Collections.Generic;

#nullable enable

/// <summary>
/// Service that responsible to shuffle and distribute index of card to each players
/// </summary>
public interface ICardShuffleService
{
    /// <summary>
    /// Require player count that the game will have.
    /// Return dictionary of player index as int and list of shuffled card index
    /// </summary>
    Dictionary<int, List<int>> Shuffle(int playerCount);

    int GetFirstTurnPlayerIndex(Dictionary<int, List<int>> shuffleResult);
}
