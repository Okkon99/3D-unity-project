using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDivideManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerPrefab;
    public PlayerMovement originalPlayer;
    public PlayerCamera playerCamera;

    [Header("Division Settings")]
    public float recombineRadius;
    public float suctionMultiplier;

    PlayerMovement bodyA;
    PlayerMovement bodyB;

    bool isDivided;

    public PlayerMovement activePlayer;
    public PlayerMovement inactivePlayer;

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
        inactivePlayer = null;

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

        if (input.Reset.triggered)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Divide()
    {
        isDivided = true;

        bodyB = Instantiate(playerPrefab, bodyA.transform.position, bodyA.transform.rotation);
        bodyB.storedPitch = bodyA.storedPitch;

        Physics.IgnoreCollision(bodyA.playerCollider, bodyB.playerCollider, true);

        // Momentum split
        Vector3 velocity = bodyA.GetComponent<Rigidbody>().linearVelocity;
        bodyB.GetComponent<Rigidbody>().linearVelocity = velocity * 2f;
        bodyA.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        var fov = mainCamera.GetComponent<MomentumFOV>();
        if (fov != null)
        {
            float strength = bodyB.GetComponent<Rigidbody>().linearVelocity.magnitude / 10f;
            fov.AddDivideImpulse(strength);
        }
 

        bodyA.SetActive(false);
        bodyB.SetActive(true);

        activePlayer = bodyB;
        inactivePlayer = bodyA;

        AttachCameraInstant(activePlayer);
    }

    void SwapBodies()
    {
        if (isCameraTransitioning)
        {
            return;
        }

        PlayerMovement previousActive = activePlayer;

        previousActive.SetActive(false);

        activePlayer = inactivePlayer;
        inactivePlayer = previousActive;

        StartCoroutine(SwapCamera(previousActive, activePlayer));
    }

    void TryRecombine()
    {
        float dist = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

        if (dist <= recombineRadius * suctionMultiplier)
        {
            inactivePlayer.transform.position = Vector3.MoveTowards(
                inactivePlayer.transform.position,
                activePlayer.transform.position,
                2f * Time.deltaTime
                );
            if (dist <= recombineRadius)
            {
                Recombine();
            }
        }
    }

    void Recombine()
    {
        isDivided = false;

        PlayerMovement survivor = activePlayer;
        PlayerMovement absorbed = inactivePlayer;

        survivor.SetActive(true);
        Destroy(absorbed.gameObject);

        bodyA = survivor;
        bodyB = null;

        activePlayer = survivor;
        inactivePlayer = null;
        originalPlayer = survivor;

        AttachCameraInstant(activePlayer);
    }



    //Camera stuff

    void AttachCameraInstant(PlayerMovement player)
    {
        mainCamera.transform.SetParent(player.cameraAnchor);
        mainCamera.transform.localPosition = cameraLocalOffset;
        mainCamera.transform.localRotation = Quaternion.identity;

        playerCamera.SetAnchor(player.cameraAnchor, player.storedPitch);

        var fov = mainCamera.GetComponent<MomentumFOV>();
        if (fov != null)
        {
            fov.SetVelocitySource(player.GetComponent<Rigidbody>());
        }
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
        playerCamera.SetAnchor(toAnchor, to.storedPitch);

        to.SetActive(true);

        isCameraTransitioning = false;

        var fov = mainCamera.GetComponent<MomentumFOV>();
        if (fov != null)
            fov.SetVelocitySource(to.GetComponent<Rigidbody>());
    }
}
