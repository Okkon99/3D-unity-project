using UnityEngine;
using UnityEngine.Rendering;

public class StickyCubeAugment : AugmentBase
{
    public bool toggleable = false;
    public override bool Toggleable => toggleable;

    private FixedJoint stickJoint;

    private bool isStuck = false;
    private bool canStick = true;


    private void OnCollisionStay(Collision collision)
    {
        if (isStuck || !canStick) return;

        int floorLayer = LayerMask.NameToLayer("Floor");
        if (collision.collider.gameObject.layer == floorLayer)
        {
            StickToSurface(collision.collider);
        }
    }

    private void StickToSurface(Collider target)
    {
        isStuck = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        stickJoint = gameObject.AddComponent<FixedJoint>();

        Rigidbody targetRB = target.attachedRigidbody;

        if (targetRB != null)
            stickJoint.connectedBody = targetRB;
    }

    public void UnStick()
    {
        if (!isStuck) return;

        if (stickJoint != null)
            Destroy(stickJoint);

        isStuck = false;
        canStick = false;
    }

    public override void OnEquipped(PlayerMovement newOwner)
    {
        base.OnEquipped(newOwner);
        UnStick();
    }

    public void OnRelease()
    {
        canStick = true;
    }

    public override void ResetToSpawn()
    {
        transform.SetParent(null);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isStuck = false;
        rb.isKinematic = false;

        transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        transform.rotation = startRotation;

        gameObject.SetActive(true);
    }
}