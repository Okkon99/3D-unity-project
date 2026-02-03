using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;

    private void OnCollisionStay(Collision collision)
    {
        raisedPlatform.OnTriggerActivatePlatform(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        raisedPlatform.OnTriggerActivatePlatform(false);
    }
}
