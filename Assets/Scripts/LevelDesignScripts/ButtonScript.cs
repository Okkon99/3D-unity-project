using System.Collections;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;
    [SerializeField] RaisedPlatform raisedPlatform2;
    [SerializeField] RaisedPlatform raisedPlatform3;
    [SerializeField] RaisedPlatform raisedPlatform4;
    [SerializeField] RaisedPlatform raisedPlatform5;
    [SerializeField] RaisedPlatform raisedPlatform6;
    [SerializeField] RaisedPlatform raisedPlatform7;
    [SerializeField] RaisedPlatform raisedPlatform8;
    [SerializeField] GateScript gateScript;
    [SerializeField] GateScript gateScript2;
    [SerializeField] GateScript gateScript3;
    [SerializeField] GateScript gateScript4;
    [SerializeField] GateScript gateScript5;
    [SerializeField] GateScript gateScript6;
    [SerializeField] GateScript gateScript7;
    [SerializeField] GateScript gateScript8;

    [SerializeField] MeshRenderer meshPath;
    [SerializeField] Material PressedMaterial;
    Material material;
    Material defaultMaterial;
    bool isPressed;

    Vector3 startPos;
    Vector3 endPos;

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer == null)
        {
            return;
        }

        material = renderer.material;
        material.EnableKeyword("_Emission");
        PressedMaterial.SetColor("_Emission_Color", Color.black);
    }


    private void Start()
    {
        defaultMaterial = GetComponent<Material>();
        startPos = transform.position;
        PressedMaterial.SetColor("_Emission_Color", Color.black);
    }

    public void OnButtonPressed()
    {
        if (isPressed)
            return;

        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(true);

        if (raisedPlatform2 != null)
            raisedPlatform2.OnTriggerActivatePlatform(true);

        if (raisedPlatform3 != null)
            raisedPlatform3.OnTriggerActivatePlatform(true);

        if (raisedPlatform4 != null)
            raisedPlatform4.OnTriggerActivatePlatform(true);

        if (raisedPlatform5 != null)
            raisedPlatform5.OnTriggerActivatePlatform(true);

        if (raisedPlatform6 != null)
            raisedPlatform6.OnTriggerActivatePlatform(true);

        if (raisedPlatform7 != null)
            raisedPlatform7.OnTriggerActivatePlatform(true);

        if (raisedPlatform8 != null)
            raisedPlatform8.OnTriggerActivatePlatform(true);


        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);

        if (gateScript2 != null)
            gateScript2.OnTriggerActivateGate(true);

        if (gateScript3 != null)
            gateScript3.OnTriggerActivateGate(true);

        if (gateScript4 != null)
            gateScript4.OnTriggerActivateGate(true);

        if (gateScript5 != null)
            gateScript5.OnTriggerActivateGate(true);

        if (gateScript6 != null)
            gateScript6.OnTriggerActivateGate(true);
        
        if (gateScript7 != null)
            gateScript7.OnTriggerActivateGate(true);
        
        if (gateScript8 != null)
            gateScript8.OnTriggerActivateGate(true);

        StartCoroutine(PressAnimation());
        material.SetColor("_Emission_Color", Color.black);
    }

    public void OnButtonExpired()
    {
        if (raisedPlatform != null)
            raisedPlatform.OnTriggerActivatePlatform(false);
        
        if (gateScript != null)
            gateScript.OnTriggerActivateGate(false);

        meshPath.material = defaultMaterial;
    }


    private IEnumerator PressAnimation()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 start = startPos;
        Vector3 target = startPos - (transform.up * 0.2f);


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        transform.position = target;

        isPressed = true;
        meshPath.material = PressedMaterial;
    }
}
