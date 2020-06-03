//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestingLocalRotation : MonoBehaviour
//{
//    //[SerializeField] float m_RotationalSpeed = default;

//    [SerializeField, Tooltip("The amount of rotation you wish the Transform to have rotated by the end of the Lerp")]
//    Vector3 m_AmountToRotate = default;

//    //[SerializeField] Vector3 m_RotationalSpeed = default;
//    [SerializeField, ReadOnly] Vector3 m_RotationalDirection = default;
//    [SerializeField] Space m_Space = default;
//    //[SerializeField] bool m_Local = default;

//    //Quaternion m_OriginalRotation = default;
//    Vector3 m_NewEulerRotation = default;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //Depends on what axis you wanna rotate on
//        m_RotationalDirection = m_Space == Space.Self ? transform.right : transform.InverseTransformDirection(transform.right);
//        //m_RotationalDirection = m_Local ? transform.right : transform.InverseTransformDirection(transform.right);

//        //m_OriginalRotation = transform.rotation;
//        m_NewEulerRotation = Vector3.zero;
//    }

//    //// Update is called once per frame
//    //void Update()
//    //{
//    //    m_NewEulerRotation = Vector3.Lerp(m_NewEulerRotation, m_AmountToRotate, 0.4f * Time.deltaTime);

//    //    if(Vector3.SqrMagnitude(m_NewEulerRotation- m_AmountToRotate) < 1f)
//    //    {
//    //        enabled = false;
//    //        return;
//    //    }

//    //    //transform.Rotate(m_NewEulerRotation, m_Space);
//    //    //transform.rotation = Quaternion.Euler(Vector3.Scale(m_NewEulerRotation,m_RotationalDirection)) * m_OriginalRotation;


//    //}
//}
