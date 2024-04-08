using System.Collections.Generic;

#nullable enable

public interface ISubmittableCombinationService
{
    void GetCombination(
        List<Card> cards,
        List<ISubmittableCard> submittables);
}
