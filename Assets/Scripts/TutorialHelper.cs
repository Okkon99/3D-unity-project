using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialHelper : MonoBehaviour
{
    [SerializeField] Tutorial tutorialPart;
    [SerializeField] Tutorial tutorialPart2;
    [SerializeField] PlayerBackpack playerBackpack1;
    [SerializeField] PlayerBackpack playerBackpack2;
    [SerializeField] bool fulfillsCondition1;


    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerMovement isPlayer;
    [SerializeField] ButtonScript button;
    [SerializeField] PressurePlate pressurePlate;




    private void Update()
    {
        if (rb != null && fulfillsCondition1 && tutorialPart.col == null)
        {
            tutorialPart.condition1 = true;
            tutorialPart.ClearText();
            rb = null;
        }

        if (isPlayer != null && isPlayer.isActivePlayer && tutorialPart.col == null)
        {
            tutorialPart.ClearText();
            isPlayer = null;
        }

        if (button != null && button.isPressed && tutorialPart.col == null)
        {
            tutorialPart.ClearText();
            button = null;
        }

        if (playerBackpack1 && playerBackpack2 != null && (playerBackpack1.IsOccupied || playerBackpack2.IsOccupied) && tutorialPart2.col == null)
        {
            tutorialPart2.text.text = "Right-Click to eject the\nactive robot's augment slot";
            
            Destroy(this);
        }

        if (pressurePlate != null && pressurePlate.isPressed && tutorialPart.col == null)
        {
            tutorialPart.ClearText();
            pressurePlate = null;
        }
    }
}
