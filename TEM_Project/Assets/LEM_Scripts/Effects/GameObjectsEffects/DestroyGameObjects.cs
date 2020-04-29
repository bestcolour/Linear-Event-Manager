using UnityEngine;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the gameobject related events for TEM
/// </summary>
namespace LEM_Effects
{
    public class DestroyGameObjects : LEM_BaseEffect
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject[] targetObjects = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Destroy the targetted objects
            for (int i = 0; i < targetObjects.Length; i++)
            {
                GameObject.Destroy(targetObjects[i]);
            }
        }


    }

   
}

