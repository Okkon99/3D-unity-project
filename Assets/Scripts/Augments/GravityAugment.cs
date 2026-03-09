using UnityEngine;

public class GravityAugment : AugmentBase
{
    [SerializeField] float cooldownTimer;
    [SerializeField] float timer;

    private void Awake()
    {
        base.Awake();

        timer = cooldownTimer;
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;


            if (owner != null && owner.IsGrounded && timer < (cooldownTimer - 0.1f))
                timer = 0;
        }
    }

    public override void Toggle()
    {


        if (timer > 0) return;

        if (owner == null) return;

        base.Toggle();

        if (isActive)
        {
            owner.SetGravityDirection(Vector3.up);
        }
        else
        {
            owner.SetGravityDirection(Vector3.down);
        }

        timer = cooldownTimer;
    }

    public override void OnUnequipped()
    {
        owner.SetGravityDirection(Vector3.down);
        base.OnUnequipped();
    }
}