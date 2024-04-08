using System.Collections.Generic;
using UnityEngine;

#nullable enable

[CreateAssetMenu(fileName = "CardCollectionData", menuName = "Data/Card Collection Data")]
public class CardCollectionScriptableObject : ScriptableObject, ICardCollection
{
    [SerializeField]
    private List<Card> _cards = new List<Card>();

    public List<Card> Cards => _cards;
}
