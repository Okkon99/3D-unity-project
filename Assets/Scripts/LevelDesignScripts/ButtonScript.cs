using System.Collections;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] RaisedPlatform raisedPlatform;
    [SerializeField] RaisedPlatform raisedPlatform2;
    [SerializeField] RaisedPlatform raisedPlatform3;
    [SerializeField] GateScript gateScript;
    [SerializeField] GateScript gateScript2;
    [SerializeField] GateScript gateScript3;

    [SerializeField] MeshRenderer meshPath;
    [SerializeField] Material PressedMaterial;

    Material defaultMaterial;
    bool isPressed;

    Vector3 startPos;
    Vector3 endPos;

    private void Start()
    {
        defaultMaterial = GetComponent<Material>();
        startPos = transform.position;
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

        if (gateScript != null)
            gateScript.OnTriggerActivateGate(true);

        if (gateScript2 != null)
            gateScript2.OnTriggerActivateGate(true);

        if (gateScript3 != null)
            gateScript3.OnTriggerActivateGate(true);

        StartCoroutine(PressAnimation());
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
