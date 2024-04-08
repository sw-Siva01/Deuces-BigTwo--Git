using System.Collections.Generic;

#nullable enable

public interface ISubmittableIdentifier
{
    bool TryGetSubmittable(
        List<Card> cards,
        out ISubmittableCard? submittableCard);
}
