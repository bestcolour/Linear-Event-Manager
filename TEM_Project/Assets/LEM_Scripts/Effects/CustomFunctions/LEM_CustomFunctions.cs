using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the CustomFunctions effects related events for TEM
/// </summary>
namespace LEM_Effects
{
    public class CustomFunctions : LEM_BaseEffect
    {
        [Tooltip("Object with the custom function/method you want to execute")]
        public UnityEvent m_CustomScriptEvent;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;
    }

}