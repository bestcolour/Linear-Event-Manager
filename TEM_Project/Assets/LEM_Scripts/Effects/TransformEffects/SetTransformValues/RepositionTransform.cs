using UnityEngine;
namespace LEM_Effects
{

    public class RepositionTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the position relative to the transform's parent. False means to set the position relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
                m_TargetTransform.localPosition = m_TargetPosition;
            else
                m_TargetTransform.position = m_TargetPosition;

        }


    } 
}