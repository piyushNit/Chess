using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

    GameplayScene gameplayScene;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private float gameOverCanvasAnimationTime = 0.5f;
    private Vector3 gameOverCanvasTargetPos;
    private Vector3 gameOverCavasDefaultPos;

    private ScreenTransManager screenTransManager;

    void Start() {
        InitMemberVars();
    }

    private void InitMemberVars() {
        gameOverCavasDefaultPos = gameOverCanvas.transform.position;
        gameOverCanvasTargetPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        screenTransManager = GameObject.FindObjectOfType<ScreenTransManager>();
    }

    public void ResetTheGame() {
        screenTransManager.ResetTheGame();
    }

    public void UndoTheSteps() {
        if (gameplayScene == null)
            SetGamePlaySceneVar();
        gameplayScene.UndoTheStep();
    }

    void SetGamePlaySceneVar() {
        gameplayScene = GameObject.FindObjectOfType<GameplayScene>();
    }

    public void ShowGameOver() {
        iTween.MoveTo(gameOverCanvas, gameOverCanvasTargetPos, gameOverCanvasAnimationTime);
    }

    public void GoToMainMenu() {
        screenTransManager.GoToMainMenuScene();
    }
}
