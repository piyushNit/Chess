using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour {
    private static GameStateManager _instance;
    public static GameStateManager instance {
        get { if (_instance == null) _instance = GameObject.FindObjectOfType<GameStateManager>();
            return _instance;
        }
    }

    public void GameComplete() {
        Debug.Log("Game complete");
    }

    public void ResetTheGame() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
