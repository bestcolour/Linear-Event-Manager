using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOverTime : MonoBehaviour
{
    [SerializeField]
    AnimationCurve m_Curve = default;


    float t = 0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Euler(m_Curve.Evaluate(t), 0, 0);

        t += Time.deltaTime;
    }
}
