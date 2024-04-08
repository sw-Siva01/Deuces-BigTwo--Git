using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

#nullable enable

public static class ListExtension
{
    public static void Shuffle<T>(this IList<T> list)
    {
        Random.InitState((int)DateTime.Now.Ticks);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}