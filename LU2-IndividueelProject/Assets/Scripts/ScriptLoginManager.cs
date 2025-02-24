using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptLoginManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SceneMenuWorld");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
