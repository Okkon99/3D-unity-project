using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerCamera : MonoBehaviour
{
    public float pitch;
    private PlayerPairManager pairManager;
    public Transform currentAnchor { get; private set; }

    private void Awake()
    {
        pairManager = FindFirstObjectByType<PlayerPairManager>();
    }

    private void Update()
    {
        if (pairManager == null)
        {
            return;
        }

        PlayerMovement activePlayer = pairManager.activePlayer;
        if (activePlayer == null || !activePlayer.isActivePlayer)
        {
            return;
        }

        pitch = activePlayer.storedPitch;

        var input = InputManager.instance.Input.Gameplay;
        float mouseY = input.Look.ReadValue<Vector2>().y * activePlayer.lookSensitivity;

        UpdatePitch(mouseY);

        activePlayer.storedPitch = pitch;
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

    public void SetAnchor(Transform newAnchor, float pitchFromPlayer)
    {
        currentAnchor = newAnchor;
        pitch = pitchFromPlayer;
        currentAnchor.localRotation= Quaternion.Euler(pitch, 0f, 0f);
    }
}
