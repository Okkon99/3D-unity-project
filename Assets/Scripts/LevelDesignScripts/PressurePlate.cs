using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;
    [SerializeField] GateScript gateScript;

    private void OnCollisionStay(Collision collision)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(true);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(false);
    }
}
