using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

#nullable enable

public class CardInteractable : MonoBehaviour, ICardInteractable, IDisposable
{
    public Card? _card;
    private Button? _button;
    private bool _isSelected;


    private IPlayerInteractionController _playerInteractionController;

    [Inject]
    public void Inject(IPlayerInteractionController playerInteractionController)
    {
        _playerInteractionController = playerInteractionController;

        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    

    private void OnClick()
    {
        if (!_playerInteractionController.IsOnPlay)
        {
            return;
        }

        if (!_isSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void SetupReference(Card card)
    {
        _card = card;
    }

    public void Select()
    {
        if (_card == null)
        {
            throw new InvalidOperationException("_card is null");
        }

        _isSelected = true;

        // TODO configure this better
        transform.position += Vector3.up * 100;

        _playerInteractionController.SelectCard(_card);
    }

    public void Deselect()
    {
        if (_card == null)
        {
            throw new InvalidOperationException("_card is null");
        }

        _isSelected = false;

        // TODO configure this better
        transform.position -= Vector3.up * 100;

        _playerInteractionController.DeselectCard(_card);
    }

    public void Dispose()
    {
        _button?.onClick.RemoveListener(OnClick);
    }
}
