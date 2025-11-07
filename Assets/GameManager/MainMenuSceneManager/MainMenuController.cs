
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void MainGameStart()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void TesterSceneStart()
    {
        SceneManager.LoadScene("TesterScene");
    }

    public void PackageScene_LowPolygonDarkFantasy_Catherdral_SceneStart()
    {
        SceneManager.LoadScene("Demo_Cathedral_01");
    }

    public void LightTestSceneStart()
    {
        SceneManager.LoadScene("LightTestScene");
    }
}
