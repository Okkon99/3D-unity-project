using UnityEngine;

public class StickyCubeAugment : AugmentBase
{
    public bool toggleable = false;
    public override bool Toggleable => toggleable;

    private bool isStuck = false;
    private Transform stuckParent;

    private void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;

        int floorLayer = LayerMask.NameToLayer("Floor");
        if (collision.collider.gameObject.layer == floorLayer)
        {
            StickToSurface(collision.collider.transform);
        }
    }

    private void StickToSurface(Transform target)
    {
        isStuck = true;
        stuckParent = target;

        rb.isKinematic = true;
        transform.SetParent(stuckParent);
    }

    public void UnStick()
    {
        if (!isStuck) return;

        isStuck = false;
        transform.SetParent(null);
        rb.isKinematic = false; 
    }

    public override void OnEquipped(PlayerMovement newOwner)
    {
        base.OnEquipped(newOwner);
        UnStick();
    }

    public override void ResetToSpawn()
    {
        transform.SetParent(null);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isStuck = false;
        stuckParent = null;
        rb.isKinematic = false;

        transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        transform.rotation = startRotation;

        gameObject.SetActive(true);
    }
}