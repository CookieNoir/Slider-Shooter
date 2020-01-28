using UnityEngine;
using UnityEngine.SceneManagement;
[AddComponentMenu("Game UI/Scene Changer")]
public class SceneChanger : MonoBehaviour
{
    public string sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
