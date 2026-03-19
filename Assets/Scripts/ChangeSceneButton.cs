using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void GoToNextScene()
    {
        SceneManager.LoadScene("Name");
    }
}
