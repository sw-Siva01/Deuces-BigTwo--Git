using System.Collections.Generic;

#nullable enable

public interface ITableController
{
    void Setup(List<IParticipantInfo> participants);
    void SubmitCard(
        ParticipantIdEnum participantId,
        ISubmittableCard submittedCard);
    void Clear();
}