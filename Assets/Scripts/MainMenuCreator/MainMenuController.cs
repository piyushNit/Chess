using UnityEngine;

public class MainMenuController : MonoBehaviour {
    [SerializeField] private string sceneName;
    ScreenTransManager screenTransmanager;

    void Start() {
        screenTransmanager = GameObject.FindObjectOfType<ScreenTransManager>();
    }

    public void PlayNewGame() {
        screenTransmanager.PlayNewGame(sceneName);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
