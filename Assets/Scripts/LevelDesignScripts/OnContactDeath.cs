using UnityEngine;
using UnityEngine.SceneManagement;

public class OnContactDeath : MonoBehaviour
{
    [SerializeField] float number;

    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
