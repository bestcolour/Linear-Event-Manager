namespace LEM_Effects
{

    // <summary>
    // This abstract class is used for effects which has been reused in the node editor to make 2 nodes effects
    // For eg. 
    // StopLinearEventNode and StopLinearEventAtNode uses the same effect : StopLinearEvent
    // PauseCurrentLinearEventNode and PauseLinearEventNode uses the effect : PauseLinearEvent
    // AddDelayNode, AddDelayAtNode , SetDelayNode , SetDelayAtNode uses AddDelayAt and SetDelayAt respectively
    // Await Input Axis or KeyCode as well as they keep a reference to the LinearEvent which they are current in
    // </summary>
    public abstract class LinkedTo_ParentLE_InstantBaseEffect : LEM_BaseEffect
    {
#if UNITY_EDITOR
        public abstract override void OnRefreshReferenceToParentLinearEvent(LinearEvent linearEvent);
#endif

    }

    public abstract class LinkedTo_ParentLE_UpdateBaseEffect : UpdateBaseEffect
    {
#if UNITY_EDITOR
        public abstract override void OnRefreshReferenceToParentLinearEvent(LinearEvent linearEvent);
#endif

    }


}