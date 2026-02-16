using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] Scene nextLevel;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("Level2");
    }
}
