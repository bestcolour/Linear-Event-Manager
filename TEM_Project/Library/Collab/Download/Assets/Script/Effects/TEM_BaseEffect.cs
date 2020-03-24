using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>
namespace TEM_Effects
{

    [Serializable]
    public abstract class TEM_BaseEffect  
    {
        //[Tooltip("The index in which the event plays out")]
        //public int eventIndex = default;

        [Tooltip("Write a summary about what this event does if you want to"), TextArea(3, 5)]
        public string description = default;


        [Tooltip("Basically holds what should be the next event to trigger")]
        public TEM_BaseEffect nextEffect = default;
     

        public virtual void Initialise()
        {        }

        //Update event for the event inheriting this
        /// <summary>
        /// Returns true when the effect has finished executing else return false to keep updating the effect
        /// </summary>
        /// <returns></returns>
        public virtual bool TEM_Update()
        { return true; }




    }


}