using System;
using System.Collections.Generic;

#nullable enable

public class ParticipantResolver
{
    private readonly ICardCollection _cardCollection;
    private readonly ITableInfo _tableInfo;
    private readonly IComputeSubmittable _computeSubmittable;
    private readonly IPlayerInteractionController _playerInteractionController;

    public ParticipantResolver(
        ICardCollection cardCollection,
        ITableInfo tableInfo,
        IComputeSubmittable computeSubmittable,
        IPlayerInteractionController playerInteractionController)
    {
        _cardCollection = cardCollection;
        _tableInfo = tableInfo;
        _computeSubmittable = computeSubmittable;
        _playerInteractionController = playerInteractionController;
    }

    public List<IParticipant> ResolveParticipants(GameModeEnum gameMode)
    {
        switch (gameMode)
        {
            case GameModeEnum.PvCom4:
                var participants = new List<IParticipant>()
                    {
                        new PlayerParticipant(
                            "Player",
                            ParticipantIdEnum.OpponentOne,
                            _cardCollection,
                            _playerInteractionController),
                        new ComputerParticipant(
                            ParticipantIdEnum.OpponentOne,
                            "Bot_1",   //Com1
                            ParticipantIdEnum.OpponentTwo,
                            _cardCollection,
                            _tableInfo,
                            _computeSubmittable),
                        new ComputerParticipant(
                            ParticipantIdEnum.OpponentTwo,
                            "Bot_2",    //Com2
                            ParticipantIdEnum.OpponentThree,
                            _cardCollection,
                            _tableInfo,
                            _computeSubmittable),
                        new ComputerParticipant(
                            ParticipantIdEnum.OpponentThree,
                            "Bot_3",    //Com3
                            ParticipantIdEnum.Player,
                            _cardCollection,
                            _tableInfo,
                            _computeSubmittable)
                        // new ComputerParticipant(
                        //     ParticipantIdEnum.OpponentFour,
                        //     "Com0",
                        //     ParticipantIdEnum.OpponentOne,
                        //     _cardCollection,
                        //     _tableInfo,
                        //     _computeSubmittable),
                    };
                return participants;
            default:
                throw new InvalidOperationException("Invalid game mode!");
        }
    }
}
