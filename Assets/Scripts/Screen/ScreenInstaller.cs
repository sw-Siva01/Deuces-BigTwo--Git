using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class ScreenInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private CardPanelCollectionView? _cardPanelCollectionView = null;

    [SerializeField]
    private ParticipantPanelCollectionView? _participantCollectionView = null;

    [SerializeField]
    private RestartResultView? _restartResultView = null;

    public void Install(IContainerBuilder builder)
    {
        if (_cardPanelCollectionView == null)
        {
            throw new InvalidOperationException("Card Collection Panel View is null!");
        }

        if (_participantCollectionView == null)
        {
            throw new InvalidOperationException("Participant Collection Panel View is null!");
        }

        if (_restartResultView == null)
        {
            throw new InvalidOperationException("_restartResultView is null");
        }

        builder.RegisterComponent<CardPanelCollectionView>(_cardPanelCollectionView);

        builder.RegisterComponent<ParticipantPanelCollectionView>(_participantCollectionView);

        builder.RegisterComponent<RestartResultView>(_restartResultView)
            .As<IResultView>();

        builder.Register<TableView>(Lifetime.Singleton)
            .As<ITableView>();

        builder.Register<ParticipantView>(Lifetime.Singleton)
            .As<IParticipantView>();
    }
}
