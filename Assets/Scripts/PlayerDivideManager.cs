using System.Collections;
using UnityEngine;

public class PlayerDivideManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerPrefab;
    public PlayerMovement originalPlayer;
    public PlayerCamera playerCamera;

    [Header("Division Settings")]
    public float recombineRadius;

    PlayerMovement bodyA;
    PlayerMovement bodyB;

    bool isDivided;

    public PlayerMovement activePlayer;

    public PlayerMovement ActivePlayer => activePlayer;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 cameraLocalOffset;
    [SerializeField] float swapCameraDuration = 0.25f;
    [SerializeField] AnimationCurve swapCameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    bool isCameraTransitioning;

    void Start()
    {
        bodyA = originalPlayer;
        activePlayer = bodyA;

        AttachCameraInstant(activePlayer);
    }

    void Update()
    {
        var input = InputManager.instance.Input.Gameplay;

        if (input.Divide.triggered && !isDivided)
        {
            Divide();
        }

        if (input.Swap.triggered && bodyA != null && bodyB != null)
        {
            SwapBodies();
        }

        if (input.Recombine.IsPressed() && isDivided)
        {
            TryRecombine();
        }
    }

    void Divide()
    {
        isDivided = true;

        bodyB = Instantiate(playerPrefab, bodyA.transform.position, bodyA.transform.rotation);

        Physics.IgnoreCollision(bodyA.playerCollider, bodyB.playerCollider, true);

        // Momentum split
        Vector3 velocity = bodyA.GetComponent<Rigidbody>().linearVelocity;
        bodyB.GetComponent<Rigidbody>().linearVelocity = velocity * 2f;
        bodyA.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
 

        bodyA.SetActive(false);
        bodyB.SetActive(true);

        activePlayer = bodyB;
        AttachCameraInstant(activePlayer);
    }

    void SwapBodies()
    {
        if (isCameraTransitioning)
        {
            return;
        }

        PlayerMovement fromPlayer = activePlayer;

        fromPlayer.SetActive(false);
        activePlayer = activePlayer == bodyA ? bodyB : bodyA;

        StartCoroutine(SwapCamera(fromPlayer, activePlayer));
    }

    void TryRecombine()
    {
        float dist = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

        if (dist <= recombineRadius)
        {
            Recombine();
        }
    }

    void Recombine()
    {
        isDivided = false;

        PlayerMovement survivor = activePlayer;
        PlayerMovement absorbed = (activePlayer == bodyA) ? bodyB : bodyA;

        survivor.SetActive(true);


        Destroy(absorbed.gameObject);

        bodyA = survivor;
        bodyB = null;

        activePlayer = survivor;
        originalPlayer = survivor;

        AttachCameraInstant(activePlayer);
    }



    //Camera stuff

    void AttachCameraInstant(PlayerMovement player)
    {
        mainCamera.transform.SetParent(player.cameraAnchor);
        mainCamera.transform.localPosition = cameraLocalOffset;
        mainCamera.transform.localRotation = Quaternion.identity;

        playerCamera.SetAnchor(player.cameraAnchor);
    }


    IEnumerator SwapCamera(PlayerMovement from, PlayerMovement to)
    {
        isCameraTransitioning = true;

        Transform cam = mainCamera.transform;
        Transform fromAnchor = from.cameraAnchor;
        Transform toAnchor = to.cameraAnchor;

        cam.SetParent(null);

        Vector3 startPos = fromAnchor.position;
        Quaternion startRot = fromAnchor.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / swapCameraDuration;
            float eased = swapCameraCurve.Evaluate(t);

            Vector3 currentEndPos = toAnchor.position;
            Quaternion currentEndRot = toAnchor.rotation;

            cam.position = Vector3.Lerp(startPos, currentEndPos, eased);
            cam.rotation = Quaternion.Slerp(startRot, currentEndRot, eased);

            yield return null;
        }

        cam.position = toAnchor.position;
        cam.rotation = toAnchor.rotation;
        cam.SetParent(toAnchor);
        playerCamera.SetAnchor(toAnchor);

        to.SetActive(true);


        isCameraTransitioning = false;
    }
}
