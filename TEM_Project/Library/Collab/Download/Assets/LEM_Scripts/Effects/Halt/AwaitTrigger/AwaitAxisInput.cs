using UnityEditor;
using UnityEngine;
namespace LEM_Effects
{

    public class AwaitAxisInput : UpdateBaseEffect,IEffectSavable<LinearEvent,SerializedObject>
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateHaltEffect;

        [SerializeField]
        AwaitAxisInputData m_AxisInputData = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        bool m_AllInputConditionsMet = false;


        public override LEM_BaseEffect ShallowClone()
        {
            AwaitAxisInput newClone = (AwaitAxisInput)MemberwiseClone();

            AwaitAxisInputData newDataInstance = ScriptableObject.CreateInstance<AwaitAxisInputData>();

            int length = m_AxisInputData.m_MoreThanAxises != null ? m_AxisInputData.m_MoreThanAxises.Length : 0;
            newDataInstance.m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_AxisInputData.m_LessThanAxises != null ? m_AxisInputData.m_LessThanAxises.Length : 0;
            newDataInstance.m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_AxisInputData.m_ApproxEqualToAxises != null ? m_AxisInputData.m_ApproxEqualToAxises.Length : 0;
            newDataInstance.m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];

            for (int i = 0; i < newDataInstance.m_MoreThanAxises.Length; i++)
                newDataInstance.m_MoreThanAxises[i] = m_AxisInputData.m_MoreThanAxises[i];

            for (int i = 0; i < newDataInstance.m_LessThanAxises.Length; i++)
                newDataInstance.m_LessThanAxises[i] = m_AxisInputData.m_LessThanAxises[i]; 
            
            for (int i = 0; i < newDataInstance.m_ApproxEqualToAxises.Length; i++)
                newDataInstance.m_ApproxEqualToAxises[i] = m_AxisInputData.m_ApproxEqualToAxises[i];


            newClone.m_AxisInputData = newDataInstance;

            return newClone;
        }

        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.AddNumberOfAwaitingInput = 1;
        }

        public override bool OnUpdateEffect(float delta)
        {
            if (m_IsFinished)
                return m_IsFinished;

            m_AllInputConditionsMet = true;

            for (int i = 0; i < m_AxisInputData.m_MoreThanAxises.Length; i++)
                if (!(Input.GetAxisRaw(m_AxisInputData.m_MoreThanAxises[i].axisName) > m_AxisInputData.m_MoreThanAxises[i].value))
                    m_AllInputConditionsMet = false;

            for (int i = 0; i < m_AxisInputData.m_LessThanAxises.Length; i++)
                if (!(Input.GetAxisRaw(m_AxisInputData.m_LessThanAxises[i].axisName) < m_AxisInputData.m_LessThanAxises[i].value))
                    m_AllInputConditionsMet = false;

            for (int i = 0; i < m_AxisInputData.m_ApproxEqualToAxises.Length; i++)
                if (!(Mathf.Approximately(Input.GetAxisRaw(m_AxisInputData.m_ApproxEqualToAxises[i].axisName), m_AxisInputData.m_ApproxEqualToAxises[i].value)))
                    m_AllInputConditionsMet = false;

            //Return when
            return m_AllInputConditionsMet;
        }

       

        public override void OnEndEffect()
        {
            base.OnEndEffect();
            m_TargetLinearEvent.AddNumberOfAwaitingInput = -1;
        }

        public void SetUp(LinearEvent t1, SerializedObject t2)
        {
            m_TargetLinearEvent = t1;
            m_AxisInputData = (AwaitAxisInputData)t2.targetObject;
        }

        public void UnPack(out LinearEvent t1, out SerializedObject t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = new SerializedObject(m_AxisInputData);
        }
    }

}