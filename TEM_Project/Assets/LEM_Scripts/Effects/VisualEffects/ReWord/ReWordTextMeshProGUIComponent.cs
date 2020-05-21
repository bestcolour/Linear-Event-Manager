﻿using UnityEngine;
using TMPro;
namespace LEM_Effects
{

    public class ReWordTextMeshProGUIComponent : LEM_BaseEffect, IEffectSavable<TextMeshProUGUI, string>
    {
        //target
        [Tooltip("The TextMeshProUGUI you want to reword")]
        [SerializeField] TextMeshProUGUI m_TargetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string m_NewText = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            //Retext 
            m_TargetText.text = m_NewText;
        }

        public void SetUp(TextMeshProUGUI t1, string t2)
        {
            m_TargetText = t1;
            m_NewText = t2;
        }

        public void UnPack(out TextMeshProUGUI t1, out string t2)
        {
            t1 = m_TargetText;
            t2 = m_NewText;
        }
    }
}