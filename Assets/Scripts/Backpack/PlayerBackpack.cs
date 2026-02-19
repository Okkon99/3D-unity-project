using Unity.VisualScripting;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] private Transform backpackAnchor;
    [SerializeField] private BackpackPreviewRenderer previewRenderer;
    [SerializeField] private PlayerGrab playerGrab;
    [SerializeField] private GameObject deploySmokePrefab;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private float smokeDuration;

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

            if (owner == pairManager.activePlayer)
                previewRenderer?.ShowItem(item);
        }

        var movement = item.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            pairManager.ForceSwapTo(GetComponent<PlayerMovement>());
            previewRenderer?.ShowItem(item);

            PlayerGrab playerGrab = movement.gameObject.GetComponent<PlayerGrab>();

            if (playerGrab.heldBody != null)
            {
                playerGrab.Release();
            }
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

        if (deploySmokePrefab != null)
        {
            GameObject smoke = Instantiate(deploySmokePrefab, (currentItem.transform.position - transform.up), Quaternion.identity);
            smoke.transform.parent = currentItem.transform;
            Destroy(smoke, smokeDuration);
        }

        currentItem = null;

        previewRenderer?.Clear();
    }


    public void RefreshPreview()
    {
        if (previewRenderer == null) return;

        if (currentItem != null)
            previewRenderer.ShowItem(currentItem);
        else
            previewRenderer.Clear();
    }
}
