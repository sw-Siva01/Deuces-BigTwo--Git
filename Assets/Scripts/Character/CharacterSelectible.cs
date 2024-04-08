using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

#nullable enable

// TODO consider other way to hold character data
public class CharacterSelectible : MonoBehaviour
{
    [SerializeField]
    private CharacterView _characterView = new();

    public CharacterView CharacterView => _characterView;

    private Button _button;
    private Action<CharacterView>? _onSelected = null;
    public void Setup(Action<CharacterView> onSelected)
    {
        _onSelected = onSelected;
    }

    private void Start()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        _onSelected?.Invoke(_characterView);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }
}
