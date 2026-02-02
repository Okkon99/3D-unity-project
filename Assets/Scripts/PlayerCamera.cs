using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerCamera : MonoBehaviour
{
    public float pitch;
    private PlayerDivideManager divideManager;
    public Transform currentAnchor { get; private set; }

    private void Awake()
    {
        divideManager = FindFirstObjectByType<PlayerDivideManager>();
    }

    private void Update()
    {
        if (divideManager == null)
        {
            return;
        }

        PlayerMovement activePlayer = divideManager.activePlayer;
        if (activePlayer == null || !activePlayer.isActivePlayer)
        {
            return;
        }

        var input = InputManager.instance.Input.Gameplay;
        float mouseY = input.Look.ReadValue<Vector2>().y * activePlayer.lookSensitivity;

        UpdatePitch(mouseY);
    }

    public void UpdatePitch(float mouseY)
    {
        if (currentAnchor == null)
        {
            return;
        }

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        currentAnchor.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void SetAnchor(Transform newAnchor)
    {
        currentAnchor = newAnchor;
        currentAnchor.localRotation= Quaternion.Euler(pitch, 0f, 0f);
    }
}
