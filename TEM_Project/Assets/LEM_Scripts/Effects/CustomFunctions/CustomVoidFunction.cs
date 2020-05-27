﻿using UnityEngine;
using UnityEditor;
using System;

namespace LEM_Effects
{
    public class CustomVoidFunction : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<SerializedObject> 
#endif
    {
        public UnityEventData m_EventObject = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override LEM_BaseEffect ShallowClone()
        {
            CustomVoidFunction dummy = (CustomVoidFunction)MemberwiseClone();
            dummy.m_EventObject = ScriptableObject.CreateInstance<UnityEventData>();

            //int delegateCount = m_EventObject.m_UnityEvent.GetPersistentEventCount();
            //ParameterInfo[] paraInfo;


            //for (int i = 0; i < delegateCount; i++)
            //{
            //    string methodName = m_EventObject.m_UnityEvent.GetPersistentMethodName(i);
            //    if (string.IsNullOrEmpty(methodName))
            //        continue;

            //    UnityEngine.Object targetReference = m_EventObject.m_UnityEvent.GetPersistentTarget(i);

            //    //Get method info of targetReference
            //    MethodInfo methodInfo = targetReference.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);



            //    //methodInfo.GetParameters()
            //    //dummy.m_EventObject.m_UnityEvent.AddListener(CreateSpecificParametersDelegateType(methodInfo) as UnityAction);
            //    dummy.m_EventObject.m_UnityEvent.AddListener(() => Delegate.CreateDelegate(typeof(UnityAction<int>), methodInfo));
            //    //dummy.m_EventObject.m_UnityEvent.AddListener(Delegate.CreateDelegate(typeof(UnityAction<>), targetReference, methodName, false) as UnityAction);
            //}


            return dummy;
        }

        //UnityAction CreateSpecificParametersDelegateType(MethodInfo methodInfo)
        //{
        //    ParameterInfo[] info = methodInfo.GetParameters();

        //    //Dont create any delegates that can handle two parameters or above cause unity doesnt serialize that shit when in inspector
        //    if (info.Length > 1)
        //        return null;

        //    switch (Type.GetTypeCode(info[0].ParameterType))
        //    {
        //        //case TypeCode.Empty:
        //            //return Delegate.CreateDelegate(typeof(UnityAction), methodInfo ) as UnityAction;

        //        case TypeCode.Object:
        //            return Delegate.CreateDelegate(typeof(UnityAction<UnityEngine.Object>), methodInfo) as UnityAction<UnityEngine.Object>;

        //        case TypeCode.String:
        //            return Delegate.CreateDelegate(typeof(UnityAction<string>), methodInfo);

        //        case TypeCode.Int32:
        //            return Delegate.CreateDelegate(typeof(UnityAction<int>), methodInfo);

        //        case TypeCode.Single:
        //            return Delegate.CreateDelegate(typeof(UnityAction<float>), methodInfo);

        //        case TypeCode.Boolean:
        //            return Delegate.CreateDelegate(typeof(UnityAction<bool>), methodInfo);

        //        default: return Delegate.CreateDelegate(typeof(UnityAction), methodInfo);

        //    }

        //}

        public override void OnInitialiseEffect()
        {
            m_EventObject.m_UnityEvent.Invoke();
        }

#if UNITY_EDITOR
        public void SetUp(SerializedObject t1)
        {
            m_EventObject = (UnityEventData)t1.targetObject;
        }

        public void UnPack(out SerializedObject t1)
        {
            t1 = new SerializedObject(m_EventObject);
        }

#endif

    }

}