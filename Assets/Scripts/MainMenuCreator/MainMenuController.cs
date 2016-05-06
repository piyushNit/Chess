using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    [SerializeField] private string sceneName;

    public void PlayNewGame() {
        SceneManager.LoadScene(sceneName);
    }
}
