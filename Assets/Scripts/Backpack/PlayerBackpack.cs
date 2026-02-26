using Unity.VisualScripting;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] private Transform backpackAnchor;
    [SerializeField] private BackpackPreviewRenderer previewRenderer;
    [SerializeField] private PlayerGrab playerGrab;
    [SerializeField] private GameObject deploySmokePrefab;
    [SerializeField] private GameObject deployBurstPrefab;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private float smokeDuration;

    private PlayerPairManager pairManager;

    private IsBackpackable currentItem;

    private PlayerGrab augmentPlayerGrab;

    public bool IsOccupied => currentItem != null;

    public void SetPairManager(PlayerPairManager manager)
    {
        pairManager = manager;
    }

    public bool TryInsert(IsBackpackable item)
    {
        if (IsOccupied) return false;
        if (!item.CanEnterBackpack()) return false;

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

            augmentPlayerGrab = movement.gameObject.GetComponent<PlayerGrab>();

            if (playerGrab.heldBody != null)
            {
                augmentPlayerGrab.Release();
            }

            augmentPlayerGrab.enabled = false;
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

        var playerMovement = currentItem.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            pairManager.ForceSwapTo(playerMovement.GetComponent<PlayerMovement>());

        if (deploySmokePrefab != null)
        {
            GameObject smoke = Instantiate(deploySmokePrefab, (currentItem.transform.position - transform.up), Quaternion.identity);
            GameObject burst = Instantiate(deployBurstPrefab, transform.position - transform.forward, Quaternion.Euler(90,0,0));
            smoke.transform.parent = currentItem.transform;
            burst.transform.parent = transform;
            Destroy(smoke, smokeDuration);
            Destroy(burst, smokeDuration);
        }

        currentItem = null;

        if (augmentPlayerGrab != null && augmentPlayerGrab.enabled == false)
            augmentPlayerGrab.enabled = true;

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
