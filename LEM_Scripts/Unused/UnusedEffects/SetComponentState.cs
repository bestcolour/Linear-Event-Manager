﻿//using UnityEngine;
//namespace LEM_Effects
//{
//    public class SetComponentState : LEM_BaseEffect
//    {
//        [Tooltip("The name of a monobehaviour script you want to enable/disable. This is case sensitive.")]
//        [SerializeField] string m_ScriptTypeName = default;

//        [Tooltip("The gameobject with the monobehaviour script you want to enable/disable")]
//        [SerializeField] GameObject m_TargetObject = default;

//        [Tooltip("True means that the script is going to be enabled, while false means disabled.")]
//        [SerializeField] bool m_State = default;

//        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

//        public override LEM_BaseEffect AddMonobehaviour(GameObject go)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override void OnInitialiseEffect()
//        {
//            //Get monobehaviour component from the targeted object
//            MonoBehaviour monobehaviourScript = m_TargetObject.GetComponent(m_ScriptTypeName) as MonoBehaviour;

//            monobehaviourScript.enabled = m_State;

//        }

//    } 
//}