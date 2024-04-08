using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

[Serializable]
public class CardView
{
    public bool IsRevealed;
    public Sprite? Sprite;
    public Sprite? BackSprite;
    public Image? CardVisualReference;
}
