using Unity.VisualScripting;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] private Transform backpackAnchor;
    private PlayerPairManager pairManager;

    private IsBackpackable currentItem;

    public bool IsOccupied => currentItem != null;

    public void SetPairManager(PlayerPairManager manager)
    {
        pairManager = manager;
    }

    public bool TryInsert(IsBackpackable item)
    {
        if (IsOccupied) return false;
        if (!item.CanEnterBackpack()) return false;
        if (item.rb.linearVelocity.y > -1f) return false;

        currentItem = item;
        item.OnEnterBackpack(backpackAnchor);

        var augment = item.GetComponent<AugmentBase>();
        if (augment != null)
        {
            var owner = GetComponent<PlayerMovement>();
            owner.EquipAugment(augment);
        }

        var movement = item.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            pairManager.ForceSwapTo(GetComponent<PlayerMovement>());
        }

        return true;
    }

    public void Deploy(Vector3 force)
    {
        if (!IsOccupied) return;

        var augment = currentItem.GetComponent<AugmentBase>();
        if (augment != null)
        {
            var owner = GetComponent<PlayerMovement>();
            owner.UnequipAugment();
        }

        currentItem.OnExitBackpack(force);
        currentItem = null;
    }

}
