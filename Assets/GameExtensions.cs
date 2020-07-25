﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameExtensions
{
    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}
