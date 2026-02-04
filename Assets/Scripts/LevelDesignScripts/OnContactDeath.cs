using UnityEngine;

public class OnContactDeath : MonoBehaviour
{
    [SerializeField] float number;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.SetActive(false);
    }
}
