using UnityEngine;
using UnityEngine.SceneManagement;


public class ScreenTransManager : MonoBehaviour {
    [SerializeField] private string mainMenuScene;

    public void GoToMainMenuScene() {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ResetTheGame() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
