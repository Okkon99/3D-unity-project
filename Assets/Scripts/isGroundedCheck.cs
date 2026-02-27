using UnityEngine;

public class isGroundedCheck : MonoBehaviour
{
    public bool isGrounded;


    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
