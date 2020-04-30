using UnityEngine;
using TMPro;
namespace LEM_Effects
{

    public class ReWordTextMeshProGUIComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The TextMeshProUGUI you want to reword")]
        [SerializeField] TextMeshProUGUI m_TargetText = default;

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