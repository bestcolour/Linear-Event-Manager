using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Namespace can be removed if u wish to use the extensions for ur own purpose :D
//and feel free to add to it!
namespace LEM_Effects.Extensions
{

    public static class TransformExtensions
    {
        public static string GetGameObjectPath(this Transform transform)
        {
            string scenePath = transform.name;

            while (transform.parent != null)
            {
                transform = transform.parent;
                scenePath = transform.name + "/" + scenePath;
            }
            return scenePath;
        }
    }

    public static class Vector3Extensions
    {
        /// <summary>
        /// Order: a/b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Divide(this Vector3 a, Vector3 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            a.z /= b.z;
            return a;
        }
    }

}