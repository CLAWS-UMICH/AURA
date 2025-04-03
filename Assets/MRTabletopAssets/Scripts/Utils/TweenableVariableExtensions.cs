using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables;

namespace UnityEngine.XR.Content.Utils
{
    public static class TweenableVariableExtensions
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public static void Initialize<T>(this TweenableVariableBase<T> variable, T value)
#pragma warning restore CS0618 // Type or member is obsolete

            where T : struct, IEquatable<T>
        {
            variable.initialValue = value;
            variable.target = value;
            variable.Value = value;
        }
    }
}
