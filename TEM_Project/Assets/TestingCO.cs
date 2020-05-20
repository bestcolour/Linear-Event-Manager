using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCO : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestingCo());
    }

    IEnumerator TestingCo()
    {
        WaitForSeconds delta = new WaitForSeconds(1f);
        //WaitForEndOfFrame delta = new WaitForEndOfFrame();

        while (true)
        {
            Debug.Log("Waiting");
            yield return delta;
        }
    }
}
