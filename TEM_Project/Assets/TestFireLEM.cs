using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFireLEM : MonoBehaviour
{
    [SerializeField,LinearDescription] LinearEvent m_Peepeepoopoo = default;

    // Start is called before the first frame update
    void Start()
    {
        m_Peepeepoopoo.InitializeLinearEvent();
        m_Peepeepoopoo.PlayLinearEvent();
    }

}
