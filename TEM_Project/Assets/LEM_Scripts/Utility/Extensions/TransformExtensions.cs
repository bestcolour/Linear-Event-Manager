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

}