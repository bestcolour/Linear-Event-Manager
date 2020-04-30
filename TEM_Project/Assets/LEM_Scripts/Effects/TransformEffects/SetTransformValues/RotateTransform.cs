using UnityEngine;
namespace LEM_Effects
{

    public class RotateTransform : LEM_BaseEffect
    {
        [Tooltip("The transform/rectransform you want to set to. Not add rotation to, but set to")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The rotation you want to set the transform to")]
        [SerializeField] Vector3 m_TargetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the rotation relative to the transform's parent. False means to set the rotation relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_TargetRotation);
            }
            else
            {
                m_TargetTransform.rotation = Quaternion.Euler(m_TargetRotation);
            }

        }

    } 
}