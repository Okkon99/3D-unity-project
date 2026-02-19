using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;
    [SerializeField] GateScript gateScript;

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

    private void OnCollisionStay(Collision collision)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(true);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);

        material.SetColor("_Emission_Color", emissionColor);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(false);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(false);

        material.SetColor("_Emission_Color", Color.black);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(true);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);

        material.SetColor("_EmissionColor", Color.red * 5f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(false);

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(false);

        material.SetColor("_EmissionColor", Color.black);
    }
}
