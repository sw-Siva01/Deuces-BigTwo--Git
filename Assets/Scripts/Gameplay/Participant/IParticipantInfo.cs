#nullable enable

public interface IParticipantInfo
{
    ParticipantIdEnum Id { get; }
    ParticipantIdEnum NextId { get; }
    string Name { get; }
    int CardCount { get; }
}
