using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float changeFieldOfView;

    private PlayerMovement player;
    private Transform cameraTransform;

    private Camera cam;

    float pitch;

    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
        cameraTransform = transform;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        { // MOUSE PITCH
            var input = InputManager.instance.Input.Gameplay;

            Vector2 lookDelta = input.Look.ReadValue<Vector2>();

            float mouseY = lookDelta.y * player.lookSensitivity;

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        } // MOUSE PITCH

        cam.fieldOfView = changeFieldOfView;

        if (!player.isActivePlayer)
        {
            
        }
    }
}
