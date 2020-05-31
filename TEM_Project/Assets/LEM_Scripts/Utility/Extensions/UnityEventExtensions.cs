#if UNITY_EDITOR
using UnityEngine.Events;

//You can remove this namespace if you wish to use this as well!s
namespace LEM_Effects.Extensions
{
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

}
#endif