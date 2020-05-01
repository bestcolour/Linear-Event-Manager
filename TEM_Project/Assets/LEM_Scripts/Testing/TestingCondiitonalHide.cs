//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestingCondiitonalHide : MonoBehaviour
//{
//    public bool m_HideConditionalField = true;

//    //[SerializeField, BoolHideIf("m_HideConditionalField",isFullPropertyPath: true,m_StateToObserve = false)]
//    [SerializeField, ConditionalReadOnly("m_HideConditionalField",  m_ConditionToMeet = false)]
//    int[] m_SlaveOne;

//    [SerializeField,ConditionalReadOnly("m_HideConditionalField",m_ConditionToMeet = false)]
//    float m_Slave2;


//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
