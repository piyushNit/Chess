using UnityEngine;
using System.Collections.Generic;

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

    public static void PrintFromList(List<Config.KillingPrioriety> killingPieces) {
        for (int i = 0; i < killingPieces.Count; ++i) {
            Config.KillingPrioriety killingPrioriety = killingPieces[i];
            GameObject myPiece = gameManager.GetObjectOnGrid(killingPrioriety.pieceGridPos);
            GameObject targetPiece = gameManager.GetObjectOnGrid(killingPrioriety.targetGridPos);
            string str = "myPiece" + myPiece.GetComponent<PieceProperties>().typeStr + ", targetPce: " + targetPiece.GetComponent<PieceProperties>().typeStr;
            Debug.Log(str);
        }
    }

}
