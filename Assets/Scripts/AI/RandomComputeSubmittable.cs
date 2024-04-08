using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

#nullable enable

public class RandomComputeSubmittable : IComputeSubmittable
{
    private ISubmittableCombinationService _submittableCombinationService;
    private ISubmittableComparator _submittableComparator;

    public RandomComputeSubmittable(
        ISubmittableCombinationService submittableCombinationService,
        ISubmittableComparator submittableComparator)
    {
        _submittableCombinationService = submittableCombinationService;
        _submittableComparator = submittableComparator;
    }

    public async UniTask<ISubmittableCard> ComputeAsync(
        List<Card> cards,
        bool isFirstTurn,
        ISubmittableCard tableSubmittable,
        CancellationToken cancellationToken)
    {
        ISubmittableCard choosenSubmittable;
        using (ListPool<ISubmittableCard>.Get(out var submittableCards))
        {
            _submittableCombinationService.GetCombination(
                cards,
                submittableCards);

            using (ListPool<ISubmittableCard>.Get(out var validSubmittable))
            {
                foreach (var submittableCard in submittableCards)
                {
                    bool isValid;
                    if (isFirstTurn)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = _submittableComparator.IsValidSubmittable(
                            tableSubmittable,
                            submittableCard);
                    }

                    if (isValid)
                    {
                        validSubmittable.Add(submittableCard);
                    }
                }

                if (validSubmittable.Count <= 0)
                {
                    choosenSubmittable = new SubmittableCard(
                        PokerHandEnum.None,
                        null!);
                }
                else
                {
                    var randomIndex = Random.Range(0, validSubmittable.Count);
                    choosenSubmittable = validSubmittable[randomIndex];
                }
            }
        }

        // TODO consider add delay from config
        await UniTask.Delay(Random.Range(5000, 7500));

        return choosenSubmittable;
    }
}
