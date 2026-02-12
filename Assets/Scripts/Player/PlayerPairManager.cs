using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerPairManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement robotA;
    [SerializeField] PlayerMovement robotB;
    [SerializeField] Camera mainCamera;
    [SerializeField] PlayerCamera playerCamera;

    [Header("Camera")]
    [SerializeField] Vector3 cameraLocalOffset;
    [SerializeField] float swapCameraDuration = 0.25f;
    [SerializeField] AnimationCurve swapCameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public PlayerMovement activePlayer;
    public PlayerMovement inactivePlayer;

    bool isCameraTransitioning;



    private void Start()
    {
        activePlayer = robotA;
        inactivePlayer = robotB;

        activePlayer.SetActive(true);
        inactivePlayer.SetActive(false);

        AttachCameraInstant(activePlayer);

        robotA.backpack.SetPairManager(this);   // had to do this to avoid using
        robotB.backpack.SetPairManager(this);   // FindFirstObjectByType<PlayerPairManager>() in the playerbackpack script
    }

    private void Update()
    {
        var input = InputManager.instance.Input.Gameplay;

        if (input.Swap.triggered)
        {
            Swap();
        }


        if (input.Reset.triggered)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Swap()
    {
        if (isCameraTransitioning)
            return;


        PlayerMovement previousActive = activePlayer;

        previousActive.SetActive(false);

        activePlayer = inactivePlayer;
        inactivePlayer = previousActive;

        StartCoroutine(SwapCamera(previousActive, activePlayer));
    }

    public void ForceSwapTo(PlayerMovement target)
    {
        if (activePlayer == target)
            return;

        if (isCameraTransitioning)
            return;

        PlayerMovement previousActive = activePlayer;

        previousActive.SetActive(false);
        
        activePlayer = target;
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
            fov.SetVelocitySource(player.GetComponent<Rigidbody>());
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

            cam.position = Vector3.Lerp(startPos, toAnchor.position, eased);

            if (t < 0.5f)
            {
                cam.rotation = startRot;
            }
            else
            {
                float rotT = Mathf.InverseLerp(0.5f, 1f, t);
                float rotEased = swapCameraCurve.Evaluate(rotT);
                cam.rotation = Quaternion.Slerp(startRot, toAnchor.rotation, rotEased);
            }

            yield return null;
        }

        cam.position = toAnchor.position;
        cam.rotation = toAnchor.rotation;
        cam.SetParent(toAnchor);

        playerCamera.SetAnchor(toAnchor, to.storedPitch);

        var fov = mainCamera.GetComponent<MomentumFOV>();
        if (fov != null)
            fov.SetVelocitySource(to.GetComponent<Rigidbody>());

        to.SetActive(true);

        isCameraTransitioning = false;
    }

}
