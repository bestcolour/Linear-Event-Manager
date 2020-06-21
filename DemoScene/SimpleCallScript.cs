using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCallScript : MonoBehaviour
{
    //Use LinearDescription attribute to make displaying LinearEvent's description easier to see
    [SerializeField,LinearDescription]
    LinearEvent m_TargetLinearEvent = default;

    private void Start()
    {
        m_TargetLinearEvent.InitializeLinearEvent();
        m_TargetLinearEvent.PlayLinearEvent();
    }


}
