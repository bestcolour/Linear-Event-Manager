using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFireLEM : MonoBehaviour
{
    [SerializeField] LinearEvent m_LinearEvent = default;

    // Start is called before the first frame update
    void Start()
    {
        m_LinearEvent.InitializeLinearEvent();
        m_LinearEvent.PlayLinearEvent();
    }

}
