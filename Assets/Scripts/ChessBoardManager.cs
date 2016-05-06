using UnityEngine;
using System.Collections;

public class ChessBoardManager : MonoBehaviour {
	[SerializeField] private GameObject chessBoard;

	private static ChessBoardManager chessBoardM;
	public static ChessBoardManager ChessBoardM {
		get { if (chessBoardM == null) chessBoardM = GameObject.FindObjectOfType<ChessBoardManager> ();
			return chessBoardM; 
		}
	}

	private Vector2 startXY;
	private Vector2 endXY;
	public Vector2 StartXY {get {return startXY;}}
	public Vector2 EndXY {get {return endXY;}}
    public float yPos;

	private float perCellDiamentionX, perCellDiamentionY;
	public float PerCellDiamentionX {get {return perCellDiamentionX;}}
	public float PerCellDiamentionY {get {return perCellDiamentionY;}}

    private GameplayScene gameplayScene;
    private ChessBoardProperties boardProperties;

	void Awake(){
        InitMemberVars();
		SetStartEndXY ();
		CalculatePerCellDiamention ();
	}

    private void InitMemberVars() {
        chessBoardM = GameObject.FindObjectOfType<ChessBoardManager>();
        gameplayScene = GameObject.FindObjectOfType<GameplayScene>();
        boardProperties = chessBoard.GetComponent<ChessBoardProperties>();
    }

	private void SetStartEndXY(){
        Vector3 childXY = boardProperties.startXY.transform.position;
        Vector3 childX = boardProperties.EndX.transform.position;
        Vector3 childY = boardProperties.endY.transform.position;

		startXY = new Vector2(childXY.x, childXY.z);
		endXY = new Vector2 (childX.x, childY.z);
        yPos = childXY.y;
	}

	private void CalculatePerCellDiamention(){
		perCellDiamentionX = Mathf.Abs((startXY.x - endXY.x) / Config.BOARD_BLOCKS);
		perCellDiamentionY = Mathf.Abs((startXY.y - endXY.y) / Config.BOARD_BLOCKS);
	}

    public void GameIsOnHold() {
        gameplayScene.canPlayTheGame = false;
    }

    public void ResumeTheGame() {
        gameplayScene.canPlayTheGame = true;
    }

    public bool IsGameplayOnHold() {
        return gameplayScene.canPlayTheGame;
    }
}
