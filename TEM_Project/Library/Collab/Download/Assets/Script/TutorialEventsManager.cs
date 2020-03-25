using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TEM_Effects;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>

public class TutorialEventsManager : MonoBehaviour
{
    public static TutorialEventsManager instance;

    //Base effects collection for node editing 
    TEM_BaseEffect[] allEffects = default;
    //Property for getting n setting alleffects
    public TEM_BaseEffect[] AllEffects
    { get { return allEffects; }
        set { AllEffects = value; }
    }


    //the currently running effects
    List<TEM_BaseEffect> playingEffects = new List<TEM_BaseEffect>();


    [Header("Initialization Checks")]
    public bool isFinishedQueuing;
    public bool isFinishedEffects;

    [Header("Effect Stats")]
    public int numOfEventsPlayin;
    public int currEventPlaying;
    public int numOfEventsLeft;

    [Header("After Effect Type")]
    public bool callingImmediateEffect;
    public bool callingNextClick;

    [Header("End of All Effects Disposal")]
    public GameObject[] endEffectDestroy;

    private void Awake()
    {
        instance = this;
        QueueEffects();
    }

    public void QueueEffects()
    {

    }



    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        for (int i = 0; i < playingEffects.Count; i++)
        {
            //If the current effect returns true,
            if (playingEffects[i].TEM_Update())
            {
                //Get a copy of the last element
                TEM_BaseEffect copy = playingEffects[0];
                //Set the effect that you want to remove as the last effect to remove it
                //and since [0] has been ran first, you dont need to worry about it running twice or none in this frame
                playingEffects[i] = copy;
                //Remove the first element
                playingEffects.RemoveAt(0);
            }

        }
    }


}
