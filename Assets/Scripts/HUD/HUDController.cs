using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

    GameplayScene gameplayScene;

    public void ResetTheGame() {
        GameStateManager.instance.ResetTheGame();
    }

    public void UndoTheSteps() {
        if (gameplayScene == null)
            SetGamePlaySceneVar();
        gameplayScene.UndoTheStep();
    }

    void SetGamePlaySceneVar() {
        gameplayScene = GameObject.FindObjectOfType<GameplayScene>();
    }
}
