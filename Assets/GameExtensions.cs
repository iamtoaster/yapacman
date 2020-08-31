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

    /// <summary>
    /// Check of object is null.
    /// </summary>
    /// <param name="obj">Object to test</param>
    /// <returns>true if null, false otherwise.</returns>
    public static bool IsNull(this object obj)
    {
        if (obj == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Play animation state and call action when it is done playing.
    /// This is a coroutine, and should be called as such.
    /// </summary>
    /// <param name="targetAnim">Animator that will play the animation</param>
    /// <param name="stateName">Name of state that will be played</param>
    /// <param name="onDone">Action to call when done</param>
    /// <returns></returns>
    public static IEnumerator PlayAndWaitForAnim(this Animator targetAnim, string stateName, Action onDone)
    {
        targetAnim.Play(stateName);

        var animHash = Animator.StringToHash(stateName);

        //Wait until we enter the current state
        while (targetAnim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash)
        {
            yield return null;
        }

        float counter = 0;
        float waitTime = targetAnim.GetCurrentAnimatorStateInfo(0).length;

        //Now, Wait until the current state is done playing
        while (counter < (waitTime))
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Done playing. Do something below!
        onDone();
    }


}
