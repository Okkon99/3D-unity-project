using System.Collections;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] TutorialPart part;

    public bool condition1;
    public Collider col;



    public enum TutorialPart
    {
        LeftClickBox,
        PressESwapCharacter,
        LeftClickButton,
        EquipAnything,
        Deploy,
        HowToMagnet
    }

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    public string GetTutorialText(TutorialPart part)
    {
        switch (part)
        {
            case TutorialPart.LeftClickBox:
                return "Left click the box to pick it up.";

            case TutorialPart.PressESwapCharacter:
                return "Press E to swap character.";

            case TutorialPart.LeftClickButton:
                return "Left click the button to press it.";
            
            case TutorialPart.EquipAnything:
                return "Grab something, then press Q to\nequip it into your augment slot.";

            case TutorialPart.Deploy:
                return "Right-Click to eject the\nactive robot's augment slot";

            case TutorialPart.HowToMagnet:
                return "Press Q to equip any held augment.\nPress Q again to toggle activation of augment.";


            default:
                return "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        text.text = GetTutorialText(part);
        Destroy(col);
    }

    public void ClearText()
    {
        text.text = "";
    }
}
