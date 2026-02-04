using UnityEngine;
using static RaisedPlatform;

public class GateScript : MonoBehaviour
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private float moveSpeed = 2f;


    [Header("Trigger Settings")]
    [SerializeField] private TriggerMode triggerMode;

    private bool triggerActive;


    public enum TriggerMode
    {
        HoldToOpen,
        PermanentAToB
    }

    public void OnTriggerActivateGate(bool active)
    {
        triggerActive = active;


       // switch (triggerMode)
       // {
        //    case TriggerMode.HoldToOpen:
       //         //just like raisedplatform's "HoldToB" movement
       //

       //     case TriggerMode.PermanentAToB:
                //make the gate move to the "Open" position forever. Does not return if some button or pressure plate makes oncollisionexit triggers a OnTriggerActiavteGate(false)
       // }
    }
}
