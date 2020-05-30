#if UNITY_EDITOR
using UnityEngine.Events;

public static class UnityEventExtensions
{
    /// <summary>
    /// Clones the specified unity event list.
    /// </summary>
    /// <param name="ev">The unity event.</param>
    /// <returns>Cloned UnityEvent</returns>
    public static T Clone<T>(this T ev) where T : UnityEventBase
    {
        return ReflectionExtensions.DeepCopy(ev);
    }

}

#endif