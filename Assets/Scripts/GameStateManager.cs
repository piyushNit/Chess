using UnityEngine;
using System.Collections;

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
}
