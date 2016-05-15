using UnityEngine;
using UnityEngine.SceneManagement;


public class ScreenTransManager : MonoBehaviour {
    [SerializeField] private string mainMenuScene;

    public void GoToMainMenuScene() {
        AnalyticsManager.instance.MainMenuOpen();
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ResetTheGame() {
        AnalyticsManager.instance.ResetBtnPressed();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayNewGame(string sceneName) {
        AnalyticsManager.instance.PlayBtnPressed();
        SceneManager.LoadScene(sceneName);
    }
}
