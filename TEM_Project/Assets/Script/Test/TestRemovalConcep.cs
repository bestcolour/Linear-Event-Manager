//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestRemovalConcep : MonoBehaviour
//{
//    public List<TestedElement> allTestedElements = default;

//    float timer = default;
//    float timerDur = 5f;

//    // Update is called once per frame
//    void Update()
//    {
//        if (timer > 0)
//        {
//            timer -= Time.deltaTime;
//        }
//        else
//        {
//            timer = timerDur;

//            for (int i = 0; i < allTestedElements.Count; i++)
//            {
//                if (allTestedElements[i].TestUpdate())
//                {
//                    //Get a copy of the last element
//                    TestedElement copy = allTestedElements[allTestedElements.Count - 1];
//                    //Set the effect that you want to remove as the last effect to remove it
//                    //and since [0] has been ran first, you dont need to worry about it running twice or none in this frame
//                    allTestedElements[i] = copy;
//                    //Remove the first element
//                    allTestedElements.RemoveAt(allTestedElements.Count - 1);
//                }
//            }
//        }


//    }
//}
