using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This can be constant rate scaling
public class LerpScaleTest : MonoBehaviour
{
    [SerializeField]
    Transform m_TargetTransform = default;

    [SerializeField]
    Vector3 m_TargetScale = default;

    [SerializeField]
    float /*m_Smoothing = 0.1f,*/ m_Duration = default;

    //[SerializeField]
    //float m_SnapRange = 0.025f;

    float m_Counter = default;
    Vector3 m_InitialScale = default;
    bool m_IsFinished = default;

    private void Start()
    {
        m_Counter = 0;
        //m_SnapRange *= m_SnapRange;
        m_IsFinished = false;
        m_InitialScale = m_TargetTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsFinished)
        {
            m_Counter += Time.deltaTime;
            float t = m_Counter / m_Duration;

            m_TargetTransform.localScale = Vector3.Lerp(m_InitialScale, m_TargetScale, t);

            if (t >= 1f)
            {
                m_IsFinished = true;
            }

        }
        ////Stop updating after target has been reached
        //if (Vector3.SqrMagnitude(m_TargetTransform.localScale - m_TargetScale) < m_SnapRange * m_SnapRange)
        //{
        //    m_TargetTransform.localScale = m_TargetScale;
        //    return true;
        //}

        
    }
}