using System.Collections;
using UnityEngine;
using static RaisedPlatform;

public class GateScript : MonoBehaviour
{
    [Header("Gate size")]
    [SerializeField] private float gateSizeX = 3f;
    [SerializeField] private float gateHeight = 3f;
    [SerializeField] private float gateSizeZ = 0.5f;

    [SerializeField] private Transform endPoint;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Trigger Settings")]
    [SerializeField] private TriggerMode triggerMode;

    public enum TriggerMode
    {
        HoldToOpen,
        PermanentOpen
    }

    private Transform gateVisual;

    private bool shouldBeOpen;
    private bool permanentlyOpened;

    private Vector3 startPos;

    private void Start()
    {
        startPos = gateVisual.position;
        endPoint.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        Vector3 target = shouldBeOpen ? endPoint.position : startPos;

        gateVisual.position = Vector3.MoveTowards(gateVisual.position, target, moveSpeed * Time.deltaTime);
    }

    private void OnValidate()
    {
        CacheChildren();
        EnsureEndPoint();
        ApplyChanges();
    }

    private void CacheChildren()
    {
        if (transform.childCount < 2)
        {
            Debug.Log("Missing children in gate prefab");
            return;
        }

        gateVisual = transform.GetChild(0);
    }

    private void EnsureEndPoint()
    {
        if (endPoint == null)
        {
            Transform existing = transform.Find("EndPoint");
            if (existing != null)
            {
                endPoint = existing;
            }
            else
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "EndPoint";
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.forward * 5f;

                endPoint = go.transform;
            }
        }
        endPoint.gameObject.SetActive(true);
        endPoint.gameObject.transform.localScale = gateVisual.localScale;
    }

    private void ApplyChanges()
    {
        if (!gateVisual)
            return;

        gateVisual.localScale = new Vector3(gateSizeX, gateHeight, gateSizeZ);
        gateVisual.localPosition = new Vector3(0f, gateHeight/2f, 0f);
    }

    public void OnTriggerActivateGate(bool active)
    {
        switch (triggerMode)
        {
            case TriggerMode.HoldToOpen:
                shouldBeOpen = active;
                break;

            case TriggerMode.PermanentOpen:
                if (!permanentlyOpened && active)
                {
                    permanentlyOpened = true;
                    shouldBeOpen = true;
                }
                break;
        }
    }
}
