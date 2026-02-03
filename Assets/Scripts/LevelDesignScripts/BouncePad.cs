using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float bouncePower = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        collision.rigidbody.AddForce(0f, bouncePower, 0f, ForceMode.Impulse);
    }

}
