using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RaisedPlatform : MonoBehaviour
{
    [Header("Platform Behaviour")]
    [SerializeField] private PlatformMovement platformMovement;
    [Header("Pillar")]
    [SerializeField] private float pillarHeight = 5f;
    [SerializeField] private float pillarWidth = 1f;
    [Header("Platform")]
    [SerializeField] private float PlatformSizeX = 5f;
    [SerializeField] private float PlatformSizeY = 1f;
    [SerializeField] private float PlatformSizeZ = 5f;

    [Header("AToB Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stayTime = 1f;
    [SerializeField] private Transform endPoint;

    [Header("Trigger Settings")]
    [SerializeField] private TriggerMode triggerMode;



    [Header("Animation Settings")]
    [SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private enum PlatformMovement
    {
        Static,
        AToB,
        TriggerActivated,
        PressureSensitive
    }

    public enum TriggerMode
    {
        HoldToB,
        OneShot,
        PermanentAToB
    }

    private Transform pillar;
    private Transform platformRoot;
    private Transform platformMesh;



    private Vector3 startPos;
    private bool movingToPoint;
    private float waitTimer;

    private bool triggerActive;
    private bool oneShotRunning;



    private void Start()
    {
        startPos = platformMesh.position;
        endPoint.GetComponent<MeshRenderer>().enabled = false;
    }


    private void Update()
    {
        switch (platformMovement)
        {
            case PlatformMovement.Static: Static(); break;

            case PlatformMovement.AToB: AToB(); break;

            case PlatformMovement.TriggerActivated: TriggerActivated(); break;

            case PlatformMovement.PressureSensitive: PressureSensitive(); break;
        }
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
            return;
        }

        pillar = transform.GetChild(0);
        platformRoot = transform.GetChild(1);
        platformMesh = platformRoot.GetChild(0);
    }

    private void EnsureEndPoint()
    {
        if (platformMovement == PlatformMovement.Static)
        {
            if (endPoint != null)
            {
                endPoint.gameObject.SetActive(false);
            }
            return;
        }

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
        endPoint.gameObject.transform.localScale = platformMesh.localScale;
    }

    private void ApplyChanges()
    {
        if (!pillar || !platformMesh)
        {
            return;
        }

        pillar.localScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);
        pillar.localPosition = new Vector3(0f, pillarHeight, 0f);

        platformMesh.localScale = new Vector3(PlatformSizeX, PlatformSizeY, PlatformSizeZ);

        platformRoot.localPosition = new Vector3(0f, pillarHeight * 2f, 0f);
    }


    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.SetParent(transform.GetChild(1));
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.gameObject.transform.SetParent(null);
    }


    private void Static()
    {
        // if im not wrong, the static state requires no method. here's one anyway that does nothing :)
    }

    private void AToB()
    {
        if (endPoint == null)
        {
            return;
        }

        Vector3 target = movingToPoint ? endPoint.position : startPos;

        platformRoot.position = Vector3.MoveTowards(platformRoot.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(platformRoot.position, target) < 0.01f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= stayTime)
            {
                waitTimer = 0f;
                movingToPoint = !movingToPoint;
            }
        }

        pillarHeight = platformRoot.transform.localPosition.y / 2f;

        pillar.localScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);
        pillar.localPosition = new Vector3(platformRoot.localPosition.x, pillarHeight, platformRoot.localPosition.z);
    }

    private void TriggerActivated()
    {
        if (endPoint == null)
        {
            return;
        }

        Vector3 target = movingToPoint ? endPoint.position : startPos;

        platformRoot.position = Vector3.MoveTowards(platformRoot.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(platformRoot.position, target) < 0.01f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= stayTime)
            {
                waitTimer = 0f;


                switch (triggerMode)
                {
                    case TriggerMode.HoldToB:
                        if (!triggerActive)
                            movingToPoint = false;
                        break;

                    case TriggerMode.OneShot:
                        if (movingToPoint)
                            movingToPoint = false;
                        else
                            oneShotRunning = false;
                        break;

                    case TriggerMode.PermanentAToB:
                        movingToPoint = !movingToPoint;
                        break;
                }
            }
        }

        pillarHeight = platformRoot.transform.localPosition.y / 2f;

        pillar.localScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);
        pillar.localPosition = new Vector3(platformRoot.localPosition.x, pillarHeight, platformRoot.localPosition.z);
    }

    private void PressureSensitive()
    {
        // fill in later
    }



    public void OnTriggerActivatePlatform(bool active)
    {
        triggerActive = active;

        if (platformMovement != PlatformMovement.TriggerActivated)
        {
            return;
        }

        switch (triggerMode)
        {
            case TriggerMode.HoldToB:
                movingToPoint = active;
                waitTimer = 0f;
                break;


            case TriggerMode.OneShot:
                if (!oneShotRunning && active)
                {
                    oneShotRunning = true;
                    movingToPoint = true;
                    waitTimer = 0f;
                }
                break;


            case TriggerMode.PermanentAToB:
                platformMovement = PlatformMovement.AToB;
                break;
        }
    }
}
