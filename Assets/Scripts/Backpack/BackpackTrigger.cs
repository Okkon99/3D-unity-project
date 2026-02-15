using Unity.VisualScripting;
using UnityEngine;

public class BackpackTrigger : MonoBehaviour
{
    private PlayerBackpack backpack;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        backpack = GetComponentInParent<PlayerBackpack>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IsBackpackable>(out var item))
        {
            backpack.TryInsert(item);
        }
    }
}
