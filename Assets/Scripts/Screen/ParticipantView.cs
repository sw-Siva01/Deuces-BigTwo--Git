using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class ParticipantView : IParticipantView
{
    private Dictionary<ParticipantIdEnum, ParticipantPanelView> _participantPanelHashMap = new();

    private readonly ParticipantPanelCollectionView _participantCollectionView;
    private readonly ITableEventListener _tableEventListener;
    private readonly ITableInfo _tableInfo;
    private readonly ICharacterSelectionService _characterSelectionService;

    public ParticipantView(
        ParticipantPanelCollectionView participantPanelCollectionView,
        ITableEventListener tableEventListener,
        ITableInfo tableInfo,
        ICharacterSelectionService characterSelectionService)
    {
        _participantCollectionView = participantPanelCollectionView;
        _tableEventListener = tableEventListener;
        _tableInfo = tableInfo;
        _characterSelectionService = characterSelectionService;

        RegisterEventCallback();
    }

    private void RegisterEventCallback()
    {
        _tableEventListener.OnCardSubmitted += OnCardSubmitted;
    }

    private void OnCardSubmitted(
        ParticipantIdEnum participantId,
        ISubmittableCard submittableCard)
    {
        var participantPanel = _participantPanelHashMap[participantId];

        var cardCount = _tableInfo.ParticipantInfoHasMap[participantId].CardCount;

        participantPanel.CardCountText!.text = cardCount.ToString();
    }

    public void Setup(List<IParticipantInfo> participantInfos)
    {
        _participantPanelHashMap.Clear();
        for (int i = 0; i < participantInfos.Count; i++)
        {
            var participantView = GetParticipantPanelView(
                i,
                participantInfos.Count);

            participantView.NameText!.text = participantInfos[i].Name;
            participantView.CardCountText!.text = participantInfos[i].CardCount.ToString();
            participantView.CharacterImage!.color = new Color(0f, 0f, 0f, 0.5f);
            participantView.cardImage!.color = new Color(0f, 0f, 0f, 0.1f); /// try ///
            participantView.playTimer!.gameObject.SetActive(false); /// try ///
            participantView.passImg!.gameObject.SetActive(false); /// try ///

            if (participantInfos[i].Id == ParticipantIdEnum.Player)
            {
                participantView.CharacterImage!.sprite = _characterSelectionService.GetSelectedCharacter().NormalExpression;
            }

            participantView.gameObject.SetActive(true);

            _participantPanelHashMap.Add(
                participantInfos[i].Id,
                participantView);
        }
    }

    public void StartTurn(ParticipantIdEnum participantId)
    {
        foreach (var participantPanel in _participantPanelHashMap.Values)
        {
            participantPanel.CharacterImage!.color = new Color(0f, 0f, 0f, 0.5f);
            participantPanel.cardImage!.color = new Color(0f, 0f, 0f, 0.1f); /// try ///
            participantPanel.playTimer!.gameObject.SetActive(false); /// try ///
            participantPanel.passImg!.gameObject.SetActive(false); /// try ///
        }

        _participantPanelHashMap[participantId].CharacterImage!.color = new Color(1f, 1f, 1f, 1f);
        _participantPanelHashMap[participantId].cardImage!.color = new Color(1f, 1f, 1f, 1f); /// try ///
        _participantPanelHashMap[participantId].playTimer!.gameObject.SetActive(true); /// try ///
        _participantPanelHashMap[participantId].passImg!.gameObject.SetActive(true); /// try ///
    }

    private ParticipantPanelView GetParticipantPanelView(
        int currentParticipantIndex,
        int participantCount)
    {
        if (participantCount == 4)
        {
            return currentParticipantIndex switch
            {
                0 => _participantCollectionView.BottomParticipantPanel!,
                1 => _participantCollectionView.RightParticipantPanel!,
                2 => _participantCollectionView.TopParticipantPanel!,
                3 => _participantCollectionView.LeftParticipantPanel!,
                _ => throw new InvalidOperationException("Invalid participant index"),
            };
        }
        else if (participantCount == 2)
        {
            return currentParticipantIndex switch
            {
                0 => _participantCollectionView.BottomParticipantPanel!,
                1 => _participantCollectionView.TopParticipantPanel!,
                _ => throw new InvalidOperationException("Invalid participant index"),
            };
        }
        else
        {
            throw new InvalidOperationException("Number of participants are not supported");
        }
    }

    public void EndGame(ParticipantIdEnum winnerParticipantId)
    {
        foreach (var participantPanel in _participantPanelHashMap)
        {
            participantPanel.Value.CharacterImage!.color = new Color(1f, 1f, 1f, 1f);
            participantPanel.Value.cardImage!.color = new Color(1f, 1f, 1f, 1f); /// try ///
            participantPanel.Value.playTimer!.gameObject.SetActive(true); /// try ///
            participantPanel.Value.passImg!.gameObject.SetActive(true); /// try ///

            if (participantPanel.Key == ParticipantIdEnum.Player)
            {
                if (winnerParticipantId == ParticipantIdEnum.Player)
                {
                    participantPanel.Value.CharacterImage!.sprite = _characterSelectionService
                        .GetSelectedCharacter().HappyExpression;
                }
                else
                {
                    participantPanel.Value.CharacterImage!.sprite = _characterSelectionService
                        .GetSelectedCharacter().SadExpression;
                }
            }
            else
            {
                if (winnerParticipantId == participantPanel.Key)
                {
                    participantPanel.Value.CharacterImage!.sprite = _characterSelectionService
                        .GetComputerCharacter().HappyExpression;
                }
                else
                {
                    participantPanel.Value.CharacterImage!.sprite = _characterSelectionService
                        .GetComputerCharacter().SadExpression;
                }
            }
        }
    }
}
