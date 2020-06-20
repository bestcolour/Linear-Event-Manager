using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFireLEM : MonoBehaviour
{
    [SerializeField,LinearDescription] LinearEvent m_Peepeepoopoo = default;
    [SerializeField,LinearDescription] LinearEvent m_Peepeepoopoo2 = default;

    // Start is called before the first frame update
    void Start()
    {
        m_Peepeepoopoo.InitializeLinearEvent();
        m_Peepeepoopoo.PlayLinearEvent();

        m_Peepeepoopoo2.InitializeLinearEvent();
        m_Peepeepoopoo2.PlayLinearEvent();
    }

}
