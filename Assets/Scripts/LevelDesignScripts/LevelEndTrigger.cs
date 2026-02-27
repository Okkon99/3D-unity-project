using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] PlayerBackpack robot1;
    [SerializeField] PlayerBackpack robot2;


    bool robot1Entered;
    bool robot2Entered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBackpack>() == robot1)
        {
            robot1Entered = true;
            if (robot2Entered || robot1.currentItem == robot1.GetComponentInParent<IsBackpackable>())
                LoadLevel();
        }
        else if (other.GetComponent<PlayerBackpack>() == robot2)
        {
            robot2Entered = true;
            if (robot1Entered || robot2.currentItem == robot1.GetComponentInParent<IsBackpackable>())
                LoadLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerBackpack>() == robot1)
            robot1Entered = false;

        if (other.GetComponent<PlayerBackpack>() == robot2)
            robot2Entered = false;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}