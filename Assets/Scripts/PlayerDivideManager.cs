using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDivideManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement player1;
    [SerializeField] PlayerMovement player2;
    [SerializeField] PlayerCamera playerCamera;

    [Header("Backpacking")]
    [SerializeField] float deployLaunchVelocity = 12f;
    [SerializeField] float collisionRestoreDistance = 1.4f;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 cameraLocalOffset;
    [SerializeField] float swapCameraDuration = 0.25f;
    [SerializeField] AnimationCurve swapCameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    PlayerMovement bodyA;
    PlayerMovement bodyB;

    public PlayerMovement activePlayer;
    public PlayerMovement inactivePlayer;

    public PlayerMovement ActivePlayer => activePlayer;

    bool isCameraTransitioning;

    BackpackState backpackedState;
    readonly List<CollisionRestoreRequest> pendingCollisionRestore = new();

    void Start()
    {
        bodyA = player1;
        bodyB = player2;

        activePlayer = bodyA;
        inactivePlayer = bodyB;

        activePlayer.SetActive(true);
        inactivePlayer.SetActive(false);

        RegisterBackpackTrigger(bodyA);
        RegisterBackpackTrigger(bodyB);

        AttachCameraInstant(activePlayer);
    }

    void Update()
    {
        var input = InputManager.instance.Input.Gameplay;

        if (input.Swap.triggered && bodyA != null && bodyB != null)
        {
            SwapBodies();
        }

        if (input.Deploy.triggered)
        {
            DeployBackpacked();
        }

        if (input.Reset.triggered)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        ProcessPendingCollisionRestore();
    }

    void RegisterBackpackTrigger(PlayerMovement player)
    {
        if (player == null || player.backpackHitbox == null)
        {
            return;
        }

        BackpackTrigger trigger = player.backpackHitbox.GetComponent<BackpackTrigger>();
        if (trigger == null)
        {
            trigger = player.backpackHitbox.gameObject.AddComponent<BackpackTrigger>();
        }

        trigger.Initialize(this, player);
    }

    public void NotifyBackpackTrigger(PlayerMovement carrier, Collider other)
    {
        if (carrier == null || other == null)
        {
            return;
        }

        Rigidbody subjectBody = other.attachedRigidbody;
        if (subjectBody == null)
        {
            return;
        }

        if (backpackedState != null)
        {
            return;
        }

        PlayerMovement subjectPlayer = subjectBody.GetComponent<PlayerMovement>();

        if (subjectPlayer == carrier)
        {
            return;
        }

        if (!CanBeBackpacked(subjectBody, subjectPlayer))
        {
            return;
        }

        if (subjectBody.linearVelocity.y >= 0f)
        {
            return;
        }

        Backpack(carrier, subjectBody, subjectPlayer);
    }

    bool CanBeBackpacked(Rigidbody subjectBody, PlayerMovement subjectPlayer)
    {
        if (subjectPlayer != null)
        {
            return subjectPlayer.canBeBackpacked;
        }

        Backpackable backpackable = subjectBody.GetComponent<Backpackable>();
        return backpackable != null && backpackable.canBeBackpacked;
    }

    void Backpack(PlayerMovement carrier, Rigidbody subjectBody, PlayerMovement subjectPlayer)
    {
        Transform backpackAnchor = carrier.backpackHitbox.transform;

        subjectBody.transform.SetParent(backpackAnchor);
        subjectBody.transform.localPosition = Vector3.zero;
        subjectBody.transform.localRotation = Quaternion.identity;

        subjectBody.linearVelocity = Vector3.zero;
        subjectBody.angularVelocity = Vector3.zero;
        subjectBody.isKinematic = true;

        if (subjectPlayer != null)
        {
            subjectPlayer.SetActive(false);
        }

        Collider[] subjectColliders = subjectBody.GetComponentsInChildren<Collider>();
        foreach (Collider col in subjectColliders)
        {
            if (col == null || carrier.playerCollider == null)
            {
                continue;
            }

            Physics.IgnoreCollision(carrier.playerCollider, col, true);
        }

        backpackedState = new BackpackState
        {
            carrier = carrier,
            subjectBody = subjectBody,
            subjectPlayer = subjectPlayer,
            ignoredColliders = subjectColliders
        };
    }

    void DeployBackpacked()
    {
        if (backpackedState == null)
        {
            return;
        }

        BackpackState state = backpackedState;
        backpackedState = null;

        state.subjectBody.transform.SetParent(null);
        state.subjectBody.isKinematic = false;

        Vector3 launchDirection = state.carrier.transform.forward + Vector3.up * 0.25f;
        state.subjectBody.linearVelocity = state.carrier.GetComponent<Rigidbody>().linearVelocity + launchDirection.normalized * deployLaunchVelocity;

        if (state.subjectPlayer != null)
        {
            state.subjectPlayer.SetActive(false);
        }

        pendingCollisionRestore.Add(new CollisionRestoreRequest
        {
            carrier = state.carrier,
            subjectBody = state.subjectBody,
            ignoredColliders = state.ignoredColliders
        });
    }

    void ProcessPendingCollisionRestore()
    {
        for (int i = pendingCollisionRestore.Count - 1; i >= 0; i--)
        {
            CollisionRestoreRequest request = pendingCollisionRestore[i];
            if (request.carrier == null || request.subjectBody == null)
            {
                pendingCollisionRestore.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(request.carrier.transform.position, request.subjectBody.position);
            if (distance < collisionRestoreDistance)
            {
                continue;
            }

            foreach (Collider collider in request.ignoredColliders)
            {
                if (collider == null || request.carrier.playerCollider == null)
                {
                    continue;
                }

                Physics.IgnoreCollision(request.carrier.playerCollider, collider, false);
            }

            pendingCollisionRestore.RemoveAt(i);
        }
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
        {
            fov.SetVelocitySource(to.GetComponent<Rigidbody>());
        }
    }

    class BackpackState
    {
        public PlayerMovement carrier;
        public Rigidbody subjectBody;
        public PlayerMovement subjectPlayer;
        public Collider[] ignoredColliders;
    }

    class CollisionRestoreRequest
    {
        public PlayerMovement carrier;
        public Rigidbody subjectBody;
        public Collider[] ignoredColliders;
    }
}
