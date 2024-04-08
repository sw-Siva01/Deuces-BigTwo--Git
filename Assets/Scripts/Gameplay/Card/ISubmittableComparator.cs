#nullable enable

public interface ISubmittableComparator
{
    bool IsValidSubmittable(
        ISubmittableCard tableSubmittableCard,
        ISubmittableCard toBeSubmittedCard);
}
