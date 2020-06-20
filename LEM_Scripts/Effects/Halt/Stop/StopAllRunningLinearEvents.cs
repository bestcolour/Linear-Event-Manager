using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")]
    public class StopAllRunningLinearEvents : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable
#endif
    {

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

        public override void OnInitialiseEffect()
        {
            LinearEventsManager.StopandResetAllRunningEvents();
        }


#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            StopLinearEvents t = go.AddComponent<StopLinearEvents>();
            t.CloneBaseValuesFrom(this);
            return t;
        }

        public void SetUp() { }

        public void UnPack() { }
#endif
    }

}