using UnityEngine;
namespace LEM_Effects
{

    public class SetGameObjectsActive : LEM_BaseEffect
    {
        [Tooltip("Objects to set their active states to true or false")]
        [SerializeField] GameObject[] targetObjects = default;

        [Tooltip("Set all the objects to this one active state")]
        [SerializeField] bool state = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Set all objects to the same state
            for (int i = 0; i < targetObjects.Length; i++)
            {
                targetObjects[i].SetActive(state);
            }
        }

    } 
}