using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum Piece{King, Queen, Bishop, Knight, Rook, Pawn};

[System.Serializable]
public struct PieceType{
	public Piece pieceType;
	public GameObject obj;
}

public class PiecesPlaceManager : MonoBehaviour {
	[Header("Please maintain the order")]
	public PieceType[] pieces;

	private GameObject piecesHolder;
	private GameManager gameManager;

    [SerializeField] private Texture whiteTexture;
    [SerializeField] private Texture darkTexture;

	void Awake(){
		InitMemberVars ();
		PlacePieces ();
    }

	void InitMemberVars(){
		piecesHolder = new GameObject ("PiecesHolder");
		gameManager = GameObject.FindObjectOfType<GameManager> ();
	}

	private void PlacePieces(){
        EditPieceProperties();
		PlaceWhitePieces ();
		PlaceBlackPieces ();
	}

	private void PlaceWhitePieces(){
		for (int i = 0; i < 16; ++i) {
			GameObject pieceObj = Instantiate(PlaceProperPiece(i), GetProperPositionForWhite(i), Quaternion.identity) as GameObject;
			pieceObj.transform.parent = piecesHolder.transform;
            pieceObj.tag = gameManager.whiteTeam;
            pieceObj.GetComponentInChildren<Renderer>().material.mainTexture = whiteTexture;
            ProperRotationForKnight(ref pieceObj);
            gameManager.SetPieceOnPosition (i, pieceObj);
		}
	}

    private void ProperRotationForKnight( ref GameObject knightObj) {
        if (knightObj.GetComponent<PieceProperties>().typeStr != Config.KNIGHT)
            return;
        knightObj.transform.eulerAngles = new Vector3(0, 180, 0);
    }

	private void PlaceBlackPieces(){
		for (int i = 0; i < 16; ++i) {
			GameObject pieceObj = Instantiate(PlaceProperPiece(i), GetProperPositionForBLack(i), Quaternion.identity) as GameObject;
            pieceObj.tag = gameManager.blackTeam;
            pieceObj.GetComponentInChildren<Renderer>().material.mainTexture = darkTexture;
            pieceObj.transform.parent = piecesHolder.transform;
			int key = (Config.TOTAL_BLOCKS -1) - i;
            gameManager.SetPieceOnPosition (key, pieceObj);
		}
	}

	private Vector3 GetProperPositionForWhite(int index){
		int yIndex = 0;
		if (index > 7) {
			index -= Config.BOARD_BLOCKS;
			yIndex = 1;
		}
		return gameManager.GetGlobalCoords (new Vector2(index, yIndex));
	}

	private Vector3 GetProperPositionForBLack(int index){
		int yIndex = 0;
		if (index > 7) {
			index -= Config.BOARD_BLOCKS;
			yIndex = 1;
		}

        float x = (ChessBoardManager.ChessBoardM.EndXY.x - ChessBoardManager.ChessBoardM.PerCellDiamentionX * index)
            - (ChessBoardManager.ChessBoardM.PerCellDiamentionX / 2);

        float z = ChessBoardManager.ChessBoardM.EndXY.y - (ChessBoardManager.ChessBoardM.PerCellDiamentionY * yIndex)
            - (ChessBoardManager.ChessBoardM.PerCellDiamentionY / 2);

        return new Vector3 (x, ChessBoardManager.ChessBoardM.yPos, z);
	}

	private GameObject PlaceProperPiece(int index){
		if(index > 7)
			return pieces [5].obj; // rook

		if (ChessRules.chessPiecesPlacement [index] == Config.KING)
			return pieces [0].obj;
		if (ChessRules.chessPiecesPlacement [index] == Config.QUEEN)
			return pieces [1].obj;
		if (ChessRules.chessPiecesPlacement [index] == Config.BISHOP)
			return pieces [2].obj;
		if (ChessRules.chessPiecesPlacement [index] == Config.KNIGHT)
			return pieces [3].obj;
		return pieces [4].obj;  //pawn
	}

    private void EditPieceProperties() {
        SetPieceProperties(ref pieces[0].obj, Config.KING);
        SetPieceProperties(ref pieces[1].obj, Config.QUEEN);
        SetPieceProperties(ref pieces[2].obj, Config.BISHOP);
        SetPieceProperties(ref pieces[3].obj, Config.KNIGHT);
        SetPieceProperties(ref pieces[4].obj, Config.ROOK);
        SetPieceProperties(ref pieces[5].obj, Config.PAWN);
    }

    private void SetPieceProperties(ref GameObject pieceObj, string type) {
        PieceProperties pieceProperty = pieceObj.GetComponent<PieceProperties>();
        pieceProperty.typeStr = type;
    }
}
