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

    public int GetBestMoves() {
        return PlayerPrefs.GetInt(Keys.KEY_BEST_MOVE);
    }

    public void SetBestMoves(int moves) {
        PlayerPrefs.SetInt(Keys.KEY_BEST_MOVE, moves);
    }

    public int GetBestScore() {
        return PlayerPrefs.GetInt(Keys.KEY_BEST_SCORE);
    }

    public void SetBestScore(int score) {
        PlayerPrefs.SetInt(Keys.KEY_BEST_SCORE, score);
    }
}
