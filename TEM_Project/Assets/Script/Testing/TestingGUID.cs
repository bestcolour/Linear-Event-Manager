using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestingGUID : MonoBehaviour
{
    [SerializeField]
    private string guidAsString;

    private System.Guid _guid;
    public System.Guid guid
    {
        get
        {

            //If current guid is empty and if inputed GUId is not empty,
            if (/*_guid == System.Guid.Empty &&*/
                 !System.String.IsNullOrEmpty(guidAsString))
            {
                //CR8 new guid using the inputted string
                _guid = new System.Guid(guidAsString);
            }
            return _guid;
        }
    }

    public void Generate()
    {
        _guid = System.Guid.NewGuid();
        guidAsString = guid.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }


}
