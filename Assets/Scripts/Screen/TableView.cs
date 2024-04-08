using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable

public class TableView : ITableView, IDisposable
{
    private readonly ICardCollection _cardCollection;
    private readonly CardPanelCollectionView _cardPanelCollectionView;
    private readonly ITableEventListener _tableEventListener;

    public TableView(
        ICardCollection cardCollection,
        CardPanelCollectionView cardPanelCollectionView,
        ITableEventListener tableEventListener)
    {
        _cardCollection = cardCollection;
        _cardPanelCollectionView = cardPanelCollectionView;
        _tableEventListener = tableEventListener;

        RegisterEventCallback();
    }

    private void RegisterEventCallback()
    {
        _tableEventListener.OnCardSubmitted += OnCardSubmitted;
        _tableEventListener.OnRoundEnded += OnRoundEnded;
    }

    private void OnRoundEnded()
    {
        foreach (var centerCardView in _cardPanelCollectionView.CenterCardPanel!.CardViews)
        {
            centerCardView.gameObject.SetActive(false);
        }
    }

    private void OnCardSubmitted(
        ParticipantIdEnum participantId,
        ISubmittableCard submittableCard)
    {
        if (submittableCard.PokerHand == PokerHandEnum.None)
        {
            return;
        }

        foreach (var centerCardView in _cardPanelCollectionView.CenterCardPanel!.CardViews)
        {
            centerCardView.gameObject.SetActive(false);
        }

        for (int i = 0; i < submittableCard.Cards.Count; i++)
        {
            var cardView = submittableCard.Cards[i].CardView;
            cardView.CardVisualReference!.gameObject.SetActive(false);
            _cardPanelCollectionView.CenterCardPanel!.CardViews[i].sprite = cardView.Sprite;
            _cardPanelCollectionView.CenterCardPanel!.CardViews[i].gameObject.SetActive(true);
        }
    }

    public async UniTask DoShuffleVisualAsync(
        Dictionary<int, List<int>> shuffleResult,
        CancellationToken cancellationToken)
    {
        foreach (var participantShuffleResult in shuffleResult)
        {
            var cardPanelView = GetCardPanelView(
                participantShuffleResult.Key,
                shuffleResult.Keys.Count);
            for (int i = 0; i < participantShuffleResult.Value.Count; i++)
            {
                if (participantShuffleResult.Key == 0)
                {
                    cardPanelView.CardViews[i].sprite = _cardCollection.Cards[participantShuffleResult.Value[i]].CardView.Sprite;
                    cardPanelView.CardViews[i].gameObject.SetActive(true);
                    // TODO make this visual config
                    await UniTask.Delay(100);
                }
                else
                {
                    cardPanelView.CardViews[i].sprite = _cardCollection.Cards[participantShuffleResult.Value[i]].CardView.Sprite;
                    cardPanelView.CardViews[i].gameObject.SetActive(true);
                    // TODO make this visual config
                    await UniTask.Delay(50);
                }
                _cardCollection.Cards[participantShuffleResult.Value[i]].CardView.CardVisualReference = cardPanelView.CardViews[i];
            }
        }
    }

    private CardPanelView GetCardPanelView(
        int currentParticipantIndex,
        int participantCout)
    {
        if (participantCout == 4)
        {
            return currentParticipantIndex switch
            {
                0 => _cardPanelCollectionView.BottomCardPanel!,
                1 => _cardPanelCollectionView.RightCardPanel!,
                2 => _cardPanelCollectionView.TopCardPanel!,
                3 => _cardPanelCollectionView.LeftCardPanel!,
                _ => throw new InvalidOperationException("Invalid participant index"),
            };
        }
        else if (participantCout == 2)
        {
            return currentParticipantIndex switch
            {
                0 => _cardPanelCollectionView.BottomCardPanel!,
                1 => _cardPanelCollectionView.TopCardPanel!,
                _ => throw new InvalidOperationException("Invalid participant index"),
            };
        }
        else
        {
            throw new InvalidOperationException("Number of participants are not supported");
        }
    }

    public void Dispose()
    {
        _tableEventListener.OnCardSubmitted -= OnCardSubmitted;
        _tableEventListener.OnRoundEnded -= OnRoundEnded;
    }
}
