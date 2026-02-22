using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;
    [SerializeField] RaisedPlatform raisedPlatform2;
    [SerializeField] RaisedPlatform raisedPlatform3;
    [SerializeField] GateScript gateScript;
    [SerializeField] GateScript gateScript2;
    [SerializeField] GateScript gateScript3;

    private Material material;
    private Color emissionColor;

    private void Awake()
    {
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();

        if (renderer == null)
        {
            return;
        }

        material = renderer.material;
        material.EnableKeyword("_Emission");
        emissionColor = material.GetColor("_Emission_Color");
        material.SetColor("_Emission_Color", Color.black);
    }

    private void OnTriggerStay(Collider other)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(true);

        if (raisedPlatform2 != null)
            raisedPlatform2.OnTriggerActivatePlatform(true);

        if (raisedPlatform3 != null)
            raisedPlatform3.OnTriggerActivatePlatform(true);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);

        if (gateScript2 != null)
            gateScript2.OnTriggerActivateGate(true);

        if (gateScript3 != null)
            gateScript3.OnTriggerActivateGate(true);

        material.SetColor("_Emission_Color", emissionColor * 5);
    }

    private void OnTriggerExit(Collider other)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(false);

        if (raisedPlatform2 != null)
            raisedPlatform2.OnTriggerActivatePlatform(false);
        
        if (raisedPlatform3 != null)
            raisedPlatform3.OnTriggerActivatePlatform(false);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(false);
        
        if (gateScript2 != null)
            gateScript2.OnTriggerActivateGate(false);
        
        if (gateScript3 != null)
            gateScript3.OnTriggerActivateGate(false);

        material.SetColor("_Emission_Color", Color.black);
    }
}
