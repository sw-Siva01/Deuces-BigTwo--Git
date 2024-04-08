using System;
using UnityEngine;

#nullable enable

[Serializable]
public class CharacterView
{
    public Sprite? NormalExpression;
    public Sprite? SadExpression;
    public Sprite? HappyExpression;

    public static implicit operator CharacterView(int v)
    {
        throw new NotImplementedException();
    }
}
