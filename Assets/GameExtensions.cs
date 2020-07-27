using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameExtensions
{
    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value) where TValue : IEquatable<TValue>
    {
        return dict.FirstOrDefault(x => x.Value.Equals(value)).Key;
    }
}
