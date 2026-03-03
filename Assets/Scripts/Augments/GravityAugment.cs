using UnityEngine;

public class GravityAugment : AugmentBase
{
    public override void Toggle()
    {
        if (owner == null) return;

        if (!owner.IsGrounded) return;

        base.Toggle();

        if (isActive)
        {
            owner.SetGravityDirection(Vector3.up);
        }
        else
        {
            owner.SetGravityDirection(Vector3.down);
        }
    }

    public override void OnUnequipped()
    {
        owner.SetGravityDirection(Vector3.down);
        base.OnUnequipped();
    }
}