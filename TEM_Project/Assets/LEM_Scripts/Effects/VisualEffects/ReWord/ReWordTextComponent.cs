using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    public class ReWordTextComponent : LEM_BaseEffect, IEffectSavable<Text, string>
    {
        //target
        [Tooltip("The Text you want to reword")]
        [SerializeField] Text m_TargetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string m_NewText = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Retext 
            m_TargetText.text = m_NewText;
        }

        public void SetUp(Text t1, string t2)
        {
            m_TargetText = t1;
            m_NewText = t2;
        }

        public void UnPack(out Text t1, out string t2)
        {
            t1 = m_TargetText;
            t2 = m_NewText;
        }
    }
}