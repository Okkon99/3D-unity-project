using UnityEngine;
using UnityEngine.XR;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float bouncePower = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null)
        {
            return;
        }

        Vector3 upDir = transform.up;

        rb.AddForce(upDir * bouncePower, ForceMode.Impulse);
    }

}
