using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLevelLoader : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SceneManager.LoadScene("DemoScene");
      
        if (Input.GetKeyDown(KeyCode.F2))
            SceneManager.LoadScene("Tutorial1");
      
        if (Input.GetKeyDown(KeyCode.F3))
            SceneManager.LoadScene("Tutorial2");
      
        if (Input.GetKeyDown(KeyCode.F4))
            SceneManager.LoadScene("Level1");
      
        if (Input.GetKeyDown(KeyCode.F5))
            SceneManager.LoadScene("Level2");
      
        if (Input.GetKeyDown(KeyCode.F6))
            SceneManager.LoadScene("Level3");
      
        if (Input.GetKeyDown(KeyCode.F7))
            SceneManager.LoadScene("Level4");
      
        if (Input.GetKeyDown(KeyCode.F8))
            SceneManager.LoadScene("Level5");
      /*
        if (Input.GetKeyDown(KeyCode.F9))
            SceneManager.LoadScene("DemoScene");
      
        if (Input.GetKeyDown(KeyCode.F10))
            SceneManager.LoadScene("DemoScene");
      
        if (Input.GetKeyDown(KeyCode.F11))
            SceneManager.LoadScene("DemoScene");
      
        if (Input.GetKeyDown(KeyCode.F12))
            SceneManager.LoadScene("DemoScene");*/

    }
}
