using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    public class ReWordTextComponent : LEM_BaseEffect
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

    } 
}