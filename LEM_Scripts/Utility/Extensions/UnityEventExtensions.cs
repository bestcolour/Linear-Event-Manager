
using UnityEngine.Events;

//You can remove this namespace if you wish to use this as well!s
namespace LEM_Effects.Extensions
{
    public static class UnityEventExtensions
    {
        /// <summary>
        /// Clones the specified unity event list.
        /// </summary>
        /// <param name="unityEventBase">The unity event.</param>
        /// <returns>Cloned UnityEvent</returns>
        public static T Clone<T>(this T unityEventBase) where T : UnityEventBase
        {
            return ReflectionExtensions.DeepCopy(unityEventBase);
        }

    }

}
