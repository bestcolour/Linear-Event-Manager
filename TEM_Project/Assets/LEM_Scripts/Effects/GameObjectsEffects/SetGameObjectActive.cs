using UnityEngine;

namespace LEM_Effects
{
    public class SetGameObjectActive : LEM_BaseEffect
    {
        [Tooltip("Object to set its active state to true or false")]
        [SerializeField] GameObject m_TargetObject = default;

        [Tooltip("Set object's active state")]
        [SerializeField] bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Set the target object to true or false
            m_TargetObject.SetActive(m_State);
        }

    } 
}