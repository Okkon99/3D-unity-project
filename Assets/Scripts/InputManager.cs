using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public InputActions Input { get; private set; }


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Input = new InputActions();
    }
    void OnEnable()
    {
        Input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        Input.Gameplay.Disable();
    }
}
