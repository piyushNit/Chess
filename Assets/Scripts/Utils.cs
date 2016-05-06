using UnityEngine;

public class Utils : MonoBehaviour{
    static GameManager gameManager;
    void Start() {
        gameManager = new GameManager();
    }
    public static void DrawTotalGrid(GameObject[] boardObjs) {
        for (int i = 0; i < boardObjs.Length; ++i) {
            Vector2 vec = gameManager.GetGridCoordsFromKeyIndex(i);
            if (boardObjs[i] != null)
                Debug.Log("x: " + vec.x + " z: " + vec.y + " type: " + boardObjs[i].GetComponent<PieceProperties>().typeStr);
            else
                Debug.Log("x: " + vec.x + vec.y);
        }
    }

}
