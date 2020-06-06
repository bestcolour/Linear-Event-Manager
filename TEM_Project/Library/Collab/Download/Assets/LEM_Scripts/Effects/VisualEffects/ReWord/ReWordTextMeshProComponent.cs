﻿using UnityEngine;
using TMPro;
namespace LEM_Effects
{


    public class ReWordTextMeshProComponent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<TextMeshPro, string> 
#endif
    {
        //target
        [Tooltip("The TextMeshPro you want to reword")]
        [SerializeField] TextMeshPro m_TargetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string m_NewText = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            //Retext 
            m_TargetText.text = m_NewText;
        }

#if UNITY_EDITOR
        public void SetUp(TextMeshPro t1, string t2)
        {
            m_TargetText = t1;
            m_NewText = t2;
        }

        public void UnPack(out TextMeshPro t1, out string t2)
        {
            t1 = m_TargetText;
            t2 = m_NewText;
        } 
#endif
    }
}