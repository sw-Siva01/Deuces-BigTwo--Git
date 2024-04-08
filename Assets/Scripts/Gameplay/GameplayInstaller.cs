using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class GameplayInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private CardCollectionScriptableObject? _cardCollectionData = null;

    public void Install(IContainerBuilder builder)
    {
        if (_cardCollectionData == null)
        {
            throw new InvalidOperationException("Card Collection Data is null! Make sure to provide the data correctly");
        }

        builder.RegisterInstance<ICardCollection>(_cardCollectionData);

        builder.RegisterEntryPoint<GameController>(Lifetime.Singleton);

        builder.Register<ParticipantResolver>(Lifetime.Singleton);

        builder.Register<CardShuffleService>(Lifetime.Singleton)
            .As<ICardShuffleService>();

        builder.Register<SubmittableCombinationService>(Lifetime.Singleton)
            .As<ISubmittableCombinationService>();

        builder.Register<SubmittableComparator>(Lifetime.Singleton)
            .As<ISubmittableComparator>();

        builder.Register<SubmittableIdentifier>(Lifetime.Singleton)
            .As<ISubmittableIdentifier>();

        builder.Register<TableMaster>(Lifetime.Singleton)
            .As<ITableController, ITableInfo, ITableEventListener>();
    }
}
