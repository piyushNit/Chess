using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    [Header("Set Team")]
    public string whiteTeam = "White";
    public string blackTeam = "Black";

    public float pieceMoveSpeed = 0.5f;

    public GameObject[] gameBoard = new GameObject[64];
    private GameplayScene gameplayScene;

    void Awake() {
        gameplayScene = GameObject.FindObjectOfType<GameplayScene>();
    }

	public void SetPieceOnPosition(int key, GameObject piece){
        for (int i = 0; i < gameBoard.Length; ++i) {
            if (gameBoard[i] == null)
                continue;
            if (gameBoard[i].GetInstanceID() == piece.GetInstanceID())
                gameBoard[i] = null;
        }
        gameBoard [key] = piece;
	}

    public void UpdatePawnPosition(GameObject obj) {
        PieceProperties pieceProperties = obj.GetComponent<PieceProperties>();
        if (!pieceProperties.isPawn)
            return;
        if (pieceProperties.canMoveTwoBlocks)
            pieceProperties.canMoveTwoBlocks = false;
    }

    /// <summary>
    /// Convert Vector3 positions to Vector2 grid positions.
    /// </summary>
    /// <param name="touchPos"></param>
    /// <returns></returns>
    public Vector2 GetGridIndex(Vector3 touchPos){
		Vector2 gridIndex = Vector2.zero;
		bool isSet = false;
		for (int x = 1; x <= Config.BOARD_BLOCKS; ++x) {
			for (int y = 1; y <= Config.BOARD_BLOCKS; ++y) {
				if (isSet)
					continue;
				if (ChessBoardManager.ChessBoardM.StartXY.x + (ChessBoardManager.ChessBoardM.PerCellDiamentionX * x) > touchPos.x &&
				    ChessBoardManager.ChessBoardM.StartXY.y + (ChessBoardManager.ChessBoardM.PerCellDiamentionY * y) > touchPos.z) {
					gridIndex = new Vector2 (x - 1, y - 1);
					isSet = true;
				}
			}
		}
		return gridIndex;
	}

    /// <summary>
    /// Get key index from vector2 gridPosition.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	public int GetIndexKey(Vector2 position){
		return (Config.BOARD_BLOCKS * (int)position.y) + (int)position.x;
	}

    /// <summary>
    /// Get object which is avaiable on perticular block
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	public GameObject GetObjectOnGrid(Vector2 position){
		int key = GetIndexKey (position);
		return gameBoard [key];
	}

    /// <summary>
    /// Get vecto2 gridPosition from given blockno or key value.
    /// </summary>
    /// <param name="keyIndex"></param>
    /// <returns></returns>
    public Vector2 GetGridCoordsFromKeyIndex(int keyIndex) {
        Vector2 gridPos = Vector2.zero;
        bool isSet = false;
        for (int x = 0; x < Config.BOARD_BLOCKS; ++x) {
            for (int y = 0; y < Config.BOARD_BLOCKS; ++y) {
                if (isSet)
                    continue;
                if ((Config.BOARD_BLOCKS * x) + (y + 1) > keyIndex) {
                        gridPos = new Vector2(x, y);
                        isSet = true;
                }
            }
        }
        return gridPos;
    }

    /// <summary>
    /// Converts vector2 gridPosition to vector3 global position
    /// </summary>
    /// <param name="gridIndex"></param>
    /// <returns></returns>
    public Vector3 GetGlobalCoords(Vector2 gridIndex){
		return new Vector3 ((ChessBoardManager.ChessBoardM.StartXY.x + ChessBoardManager.ChessBoardM.PerCellDiamentionX * gridIndex.x)
			+ (ChessBoardManager.ChessBoardM.PerCellDiamentionX / 2),
            ChessBoardManager.ChessBoardM.yPos,
			ChessBoardManager.ChessBoardM.StartXY.y + (ChessBoardManager.ChessBoardM.PerCellDiamentionY * gridIndex.y)
			+ (ChessBoardManager.ChessBoardM.PerCellDiamentionY / 2));
	}

    public Transform GetSelectablePieceTransform(int index) {
        return gameBoard[index] != null ? gameBoard[index].transform : null;
    }

    public List<Vector2> GetPossibleMoves(Vector2 pieceGridPos, bool supporting) {
        string pieceType = GetPieceType(pieceGridPos);
        if (pieceType == Config.KING)
            return GetKingMoves(pieceGridPos, supporting);
        if (pieceType == Config.QUEEN)
            return GetQueenMoves(pieceGridPos, supporting);
        if (pieceType == Config.ROOK)
            return GetRookMoves(pieceGridPos, supporting);
        if (pieceType == Config.BISHOP)
            return GetBishopMoves(pieceGridPos, supporting);
        if (pieceType == Config.KNIGHT)
            return GetKnightMoves(pieceGridPos, supporting);

        return GetPawnMoves(pieceGridPos, supporting);
    }

    public List<Vector2> GetKingMoves(Vector2 pieceGridPos, bool supporting) {
        return GetHorizontalVerticalDigonalMoves(Config.KING_MOVE, pieceGridPos, supporting);
    }

    private List<Vector2> GetQueenMoves(Vector2 pieceGridPos, bool supporting) {
        return GetHorizontalVerticalDigonalMoves(Config.QUEEN_MOVE, pieceGridPos, supporting);
    }

    private List<Vector2> GetRookMoves(Vector2 pieceGridPos, bool supporting){
        return GetHorizontalVerticalMoves(Config.ROOK_MOVE, pieceGridPos, supporting);
    }

    private List<Vector2> GetBishopMoves(Vector2 pieceGridPos, bool supporting) {
        return GetDiagonalMoves(Config.BISHOP_MOVE, pieceGridPos, supporting);
    }

    private List<Vector2> GetKnightMoves(Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        possibleMoves.AddRange(GetHorizontalKnightMoves(pieceGridPos, supporting));
        possibleMoves.AddRange(GetVerticalKnightMoves(pieceGridPos, supporting));
        return possibleMoves;
    }

    private List<Vector2> GetVerticalKnightMoves(Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        List<Vector2> tempMoves = new List<Vector2>();
        int posY1 = (int)pieceGridPos.y + Config.KNIGHT_MOVE;
        int posY2 = (int)pieceGridPos.y - Config.KNIGHT_MOVE;

        tempMoves.Add(new Vector2(pieceGridPos.x + 1, posY1));
        tempMoves.Add(new Vector2(pieceGridPos.x - 1, posY1));
        tempMoves.Add(new Vector2(pieceGridPos.x + 1, posY2));
        tempMoves.Add(new Vector2(pieceGridPos.x - 1, posY2));

        SetValidMovesFromTemp(ref possibleMoves, tempMoves, pieceGridPos, supporting);
        return possibleMoves;
    }

    private List<Vector2> GetHorizontalKnightMoves(Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        List<Vector2> tempMoves = new List<Vector2>();
        int posX1 = (int)pieceGridPos.x + Config.KNIGHT_MOVE;
        int posX2 = (int)pieceGridPos.x - Config.KNIGHT_MOVE;

        tempMoves.Add(new Vector2(posX1, pieceGridPos.y + 1));
        tempMoves.Add(new Vector2(posX1, pieceGridPos.y - 1));
        tempMoves.Add(new Vector2(posX2, pieceGridPos.y + 1));
        tempMoves.Add(new Vector2(posX2, pieceGridPos.y - 1));

        SetValidMovesFromTemp(ref possibleMoves, tempMoves, pieceGridPos, supporting);
        return possibleMoves;
    }

    private void SetValidMovesFromTemp(ref List<Vector2> possibleMoves, List<Vector2> tempMoves, Vector2 pieceGridPos, bool supporting) {
        for (int i = 0; i < tempMoves.Count; ++i) {
            if (!IsValidIndex(tempMoves[i]))
                continue;
            GameObject obj = GetObjectOnGrid(tempMoves[i]);
            if (obj == null && !supporting)
                possibleMoves.Add(tempMoves[i]);
            else {
                if (IsEnemyOrSupportingPiece(pieceGridPos, tempMoves[i], supporting))
                    possibleMoves.Add(tempMoves[i]);
            }
        }
    }

    private List<Vector2> GetPawnMoves(Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        GameObject pawnObj = GetObjectOnGrid(pieceGridPos);
        bool isWhitePawn = (pawnObj.tag == Config.WHITE_TAG);
        int pawnMoves = GetPawnMovingBlocks(pawnObj);
        for (int i = 1; i <= pawnMoves; ++i) {
            if (isWhitePawn) {
                if ((pieceGridPos.y + (i + 1)) > Config.BOARD_BLOCKS)
                    return possibleMoves;
                Vector2 piecePos = new Vector2(pieceGridPos.x, pieceGridPos.y + i);
                if (!CheckAndSetPawnMoves(ref possibleMoves, piecePos, supporting))
                    break;
            }
            else {
                if ((pieceGridPos.y - i) < 0)
                    return possibleMoves;
                Vector2 piecePos = new Vector2(pieceGridPos.x, pieceGridPos.y - i);
                if (!CheckAndSetPawnMoves(ref possibleMoves, piecePos, supporting))
                    break;
            }
        }
        PawnKillingMoves(ref possibleMoves, pieceGridPos, isWhitePawn, supporting);
        return possibleMoves;
    }

    private bool CheckAndSetPawnMoves(ref List<Vector2> possibleMoves, Vector2 pieceGridPos, bool supporting) {
        GameObject obj1 = GetObjectOnGrid(pieceGridPos);
        if (obj1 == null && !supporting)
            possibleMoves.Add(pieceGridPos);
        else
            return false;
        return true;
    }

    private void PawnKillingMoves(ref List<Vector2> possieMoves, Vector2 pieceGridPos, bool isWhitePawn, bool supporting) {
        int pos1X = (int)pieceGridPos.x - 1;
        int pos2X = (int)pieceGridPos.x + 1;
        int posY = isWhitePawn ? ((int)pieceGridPos.y + 1) : ((int)pieceGridPos.y - 1);

        bool leftKill = CheckPawnCanKill(new Vector2(pos1X, posY), pieceGridPos, supporting);
        bool rightKill = CheckPawnCanKill(new Vector2(pos2X, posY), pieceGridPos, supporting);

        if (leftKill)
            possieMoves.Add(new Vector2(pos1X, posY));
        if(rightKill)
            possieMoves.Add(new Vector2(pos2X, posY));
    }

    private bool CheckPawnCanKill(Vector2 gridPos, Vector2 pieceGridPos, bool supporting) {
        if (!IsValidIndex(gridPos))
            return false;

        GameObject obj1 = GetObjectOnGrid(gridPos);
        if (obj1 == null)
            return false;
        if (IsEnemyOrSupportingPiece(pieceGridPos, gridPos, supporting))
            return true;
        return false;
    }

    private int GetPawnMovingBlocks(GameObject pawnObj) {
        PieceProperties pieceProperties = pawnObj.GetComponent<PieceProperties>();
        bool canMoveTwoBlocks = pieceProperties.canMoveTwoBlocks;
        return canMoveTwoBlocks ? Config.PAWN_FIRST_TIME_MOVE : Config.PAWN_MOVE;
    }

    private List<Vector2> GetHorizontalVerticalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(GetHorizontalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetVerticalMoves(maxMoves, pieceGridPos, supporting));
        return possibleMoves;
    }

    private List<Vector2> GetDiagonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        possibleMoves.AddRange(GetTopLeftDiagonalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetTopRightDiagonalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetBottomLeftDiagonalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetBottomRightDiagonalMoves(maxMoves, pieceGridPos, supporting));
        return possibleMoves;
    }

    private List<Vector2> GetTopLeftDiagonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        int startX = (int)pieceGridPos.x - 1;
        int endX = ((int)pieceGridPos.x - maxMoves) <= 0 ? 0 : (int)pieceGridPos.x - maxMoves;
        int startY = (int)pieceGridPos.y + 1;
        int endY = ((int)pieceGridPos.y + maxMoves) >= Config.BOARD_BLOCKS - 1 ? Config.BOARD_BLOCKS - 1 : (int)pieceGridPos.y + maxMoves;

        while (startX >= endX && startY <= endY) {
            GameObject obj = GetObjectOnGrid(new Vector2(startX, startY));
            if (obj == null && !supporting)
                possibleMoves.Add(new Vector2(startX, startY));
            else {
                Vector2 currentPos = new Vector2(startX, startY);
                if (IsEnemyOrSupportingPiece(pieceGridPos, currentPos, supporting))
                    possibleMoves.Add(new Vector2(startX, startY));
                if(!supporting)
                    break;
            }
            startX--;
            startY++;
        }
        return possibleMoves;
    }

    private List<Vector2> GetTopRightDiagonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        int startX = (int)pieceGridPos.x + 1;
        int endX = ((int)pieceGridPos.x + maxMoves) >= Config.BOARD_BLOCKS - 1 ? Config.BOARD_BLOCKS - 1 : (int)pieceGridPos.x + maxMoves;
        int startY = (int)pieceGridPos.y + 1;
        int endY = ((int)pieceGridPos.y + maxMoves) >= Config.BOARD_BLOCKS - 1 ? Config.BOARD_BLOCKS - 1 : (int)pieceGridPos.y + maxMoves;

        while (startX <= endX && startY <= endY) {
            GameObject obj = GetObjectOnGrid(new Vector2(startX, startY));
            if (obj == null && !supporting)
                possibleMoves.Add(new Vector2(startX, startY));
            else {
                Vector2 currentPos = new Vector2(startX, startY);
                if (IsEnemyOrSupportingPiece(pieceGridPos, currentPos, supporting))
                    possibleMoves.Add(new Vector2(startX, startY));
                if(!supporting)
                    break;
            }
            startX++;
            startY++;
        }
        return possibleMoves;
    }

    private List<Vector2> GetBottomLeftDiagonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        int startX = (int)pieceGridPos.x - 1;
        int endX = ((int)pieceGridPos.x - maxMoves) <= 0 ? 0 : (int)pieceGridPos.x - maxMoves;
        int startY = (int)pieceGridPos.y - 1;
        int endY = ((int)pieceGridPos.y - maxMoves) <= 0 ? 0 : (int)pieceGridPos.y - maxMoves;

        while (startX >= endX && startY >= endY) {
            GameObject obj = GetObjectOnGrid(new Vector2(startX, startY));
            if (obj == null && !supporting)
                possibleMoves.Add(new Vector2(startX, startY));
            else {
                Vector2 currentPos = new Vector2(startX, startY);
                if (IsEnemyOrSupportingPiece(pieceGridPos, currentPos, supporting))
                    possibleMoves.Add(new Vector2(startX, startY));
                if(!supporting)
                    break;
            }
            startX--;
            startY--;
        }
        return possibleMoves;
    }

    private List<Vector2> GetBottomRightDiagonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();
        int startX = (int)pieceGridPos.x + 1;
        int endX = ((int)pieceGridPos.x + maxMoves) >= Config.BOARD_BLOCKS - 1 ? Config.BOARD_BLOCKS - 1 : (int)pieceGridPos.x + maxMoves;
        int startY = (int)pieceGridPos.y - 1;
        int endY = ((int)pieceGridPos.y - maxMoves) <= 0 ? 0 : (int)pieceGridPos.y - maxMoves;

        while (startX <= endX && startY >= endY) {
            GameObject obj = GetObjectOnGrid(new Vector2(startX, startY));
            if (obj == null && !supporting)
                possibleMoves.Add(new Vector2(startX, startY));
            else {
                Vector2 currentPos = new Vector2(startX, startY);
                if (IsEnemyOrSupportingPiece(pieceGridPos, currentPos, supporting))
                    possibleMoves.Add(new Vector2(startX, startY));
                if(!supporting)
                    break;
            }
            startX++;
            startY--;
        }
        return possibleMoves;
    }

    private List<Vector2> GetHorizontalVerticalDigonalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(GetHorizontalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetVerticalMoves(maxMoves, pieceGridPos, supporting));
        possibleMoves.AddRange(GetDiagonalMoves(maxMoves, pieceGridPos, supporting));
        return possibleMoves;
    }

    public List<Vector2> GetHorizontalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> moves = new List<Vector2>();
        int startX = 0;
        int endX = Config.BOARD_BLOCKS;
        if (maxMoves != Config.BOARD_BLOCKS) {
            startX = GetStartXPos(maxMoves, pieceGridPos);
            endX = GetEndXPos(maxMoves, pieceGridPos);
        }
        for (; startX < endX; ++startX) {
            if (startX == pieceGridPos.x)   // same piece
                continue;
            if (!AddItemsInHoriMovesArray(ref moves, new Vector2(startX, pieceGridPos.y), pieceGridPos, supporting))
                break;
        }
        return moves;
    }

    private int GetStartXPos(int maxMoves,  Vector2 pieceGridPos) {
        return (pieceGridPos.x - maxMoves) <= 0 ? 0 : (int)pieceGridPos.x - maxMoves;
    }

    private int GetEndXPos(int maxMoves, Vector2 pieceGridPos) {
        return (pieceGridPos.x + maxMoves) >= Config.BOARD_BLOCKS ? Config.BOARD_BLOCKS : (int)pieceGridPos.x + maxMoves + 1;
    }

    private bool AddItemsInHoriMovesArray(ref List<Vector2> moves, Vector2 currXY, Vector2 pieceGridPos, bool supporting) {
        int index = GetIndexKey(currXY);

        if (gameBoard[index] != null) {
            if (IsLesserThanSelectedPiece(currXY.x, pieceGridPos.x)) {
                moves.Clear();
                if (IsEnemyOrSupportingPiece(pieceGridPos, currXY, supporting))
                    moves.Add(currXY);
            }
            else {
                if (IsEnemyOrSupportingPiece(pieceGridPos, currXY, supporting))
                    moves.Add(currXY);
                return false;
            }
        }
        else {
            if (supporting)
                return false;
            moves.Add(currXY);
        }
        return true;
    }

    private bool IsEnemyOrSupportingPiece(Vector2 pieceGridPos, Vector2 currentGridPos, bool supporting) {
        bool isSupporting = IsSupportingThePiece(currentGridPos, supporting);
        if (supporting)
            return isSupporting;
        bool isEnemy = CheckForEnemyPiece(pieceGridPos, currentGridPos);
        return isEnemy;
    }

    private bool IsSameObject(GameObject obj1, GameObject obj2) {
        return obj1.tag == obj2.tag;
    }

    private bool IsLesserThanSelectedPiece(float val1, float val2) {
        return val1 < val2;
    }

    public List<Vector2> GetVerticalMoves(int maxMoves, Vector2 pieceGridPos, bool supporting) {
        List<Vector2> moves = new List<Vector2>();
        int startY = 0;
        int endY = Config.BOARD_BLOCKS;
        if (maxMoves != Config.BOARD_BLOCKS) {
            startY = GetStartYPos(maxMoves, pieceGridPos);
            endY = GetEndYPos(maxMoves, pieceGridPos);
        }
        for (; startY < endY; ++startY) {
            if (startY == pieceGridPos.y)
                continue;
            if (!AddItemsInVerticalMovesArray(ref moves, new Vector2(pieceGridPos.x, startY), pieceGridPos, supporting))
                break;
        }
        return moves;
    }

    private int GetStartYPos(int maxMoves, Vector2 pieceGridPos) {
        return (pieceGridPos.y - maxMoves) <= 0 ? 0 : (int)pieceGridPos.y - maxMoves;
    }

    private int GetEndYPos(int maxMoves, Vector2 pieceGridPos) {
        return (pieceGridPos.y + maxMoves) >= Config.BOARD_BLOCKS ? Config.BOARD_BLOCKS : (int)pieceGridPos.y + maxMoves + 1;
    }

    private bool AddItemsInVerticalMovesArray(ref List<Vector2> moves, Vector2 currXY, Vector2 pieceGridPos, bool supporting) {
        int key = GetIndexKey(currXY);

        if (gameBoard[key] != null) {
            if (IsLesserThanSelectedPiece(currXY.y, pieceGridPos.y)) {
                moves.Clear();
                if (CheckForEnemyPiece(pieceGridPos, currXY) || IsSupportingThePiece(pieceGridPos, supporting))
                    moves.Add(currXY);
            }
            else {
                if (IsEnemyOrSupportingPiece(pieceGridPos, currXY, supporting))
                    moves.Add(currXY);
                return false;
            }
        }
        else
            moves.Add(currXY);
        return true;
    }

    private bool CheckForEnemyPiece(Vector2 playerGridPos, Vector2 currGridPos) {
        GameObject playerObj = GetObjectOnGrid(playerGridPos);
        GameObject currobj = GetObjectOnGrid(currGridPos);
        return (playerObj.transform.tag != currobj.transform.tag);
    }

    private string GetPieceType(Vector2 gridPos) {
        GameObject pieceObj = GetObjectOnGrid(gridPos);
        return pieceObj.GetComponent<PieceProperties>().typeStr;
    }

    private bool IsValidIndex(Vector2 gridPosition) {
        if (((int)gridPosition.x > Config.BOARD_BLOCKS - 1) || ((int)gridPosition.y > Config.BOARD_BLOCKS - 1)
            || ((int)gridPosition.x < 0) || ((int)gridPosition.y < 0))
            return false;
        return true;
    }

    public bool CanKillPiece(Vector2 gridIndex) {
        if (GetObjectOnGrid(gridIndex) == null)
            return false;
        return true;
    }

    //AI code from here
    public List<Vector2> GetMovablePieceGridIndex(string tag, bool supporting) {
        List<Vector2> movableObjects = new List<Vector2>();
        List<GameObject> availablePieces = GetAvailablePieces(tag);
        for (int i = 0; i < availablePieces.Count; ++i) {
            Vector2 gridPos = GetGridIndex(availablePieces[i].transform.position);
            if (supporting && gameplayScene.aiKingCheckmateByPlayerPiece == gridPos)
                continue;
            List<Vector2> possibleMoves = GetPossibleMoves(gridPos, supporting);
            if (possibleMoves.Count <= 0)
                continue;
            movableObjects.Add(gridPos);
        }
        return movableObjects;
    }

    /// <summary>
    /// Return available game objects which are not dead or which can play.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public List<GameObject> GetAvailablePieces(string tag) {
        GameObject[] AiObjects = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> availableObjects = new List<GameObject>();
        for (int i = 0; i < AiObjects.Length; ++i) {
            PieceProperties pieceProperties = AiObjects[i].GetComponent<PieceProperties>();
            if (pieceProperties.isDead)
                continue;
            availableObjects.Add(AiObjects[i]);
        }
        return availableObjects;
    }

    /// <summary>
    /// Return available game objects which are not dead or which can play except king.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public List<GameObject> GetAvailablePiecesWithoutKing(string tag)
    {
        GameObject[] AiObjects = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> availableObjects = new List<GameObject>();
        for (int i = 0; i < AiObjects.Length; ++i)
        {
            PieceProperties pieceProperties = AiObjects[i].GetComponent<PieceProperties>();
            if (pieceProperties.isDead)
                continue;
            if (pieceProperties.typeStr == Config.KING)
                continue;
            availableObjects.Add(AiObjects[i]);
        }
        return availableObjects;
    }

    public void UpdatePieceProperties(Vector2 pieceGridPos) {
        GameObject obj = GetObjectOnGrid(pieceGridPos);
        PieceProperties pieceProperties = obj.GetComponent<PieceProperties>();
        pieceProperties.isDead = true;
    }

    public bool IsWhiteTeamPiece(GameObject obj) {
        return (obj.tag == Config.WHITE_TAG);
    }

    /// <summary>
    /// Search and returns object of the piece.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tagName"></param>
    /// <returns></returns>
    public GameObject GetObjectByString(string name, string tagName) {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tagName);
        GameObject piece = gameObjects[0];
        for (int i = 0; i < gameObjects.Length; ++i) {
            bool isSameName = IsSameObject(gameObjects[i], name);
            if (!isSameName)
                continue;
            piece = gameObjects[i];
            break;
        }
        return piece;
    }

    /// <summary>
    /// Search and returns Vector2 grid position of the piece.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public Vector2 GetPieceGridPosByString(string name, string tag) {
        GameObject kingPiece = GetObjectByString(name, tag);
        return GetGridIndex(kingPiece.transform.position);
    }

    /// <summary>
    /// Checks and return boolean whether that is same or not.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsSameObject(GameObject piece, string name) {
        return piece.GetComponent<PieceProperties>().typeStr == name;
    }

    /// <summary>
    /// Converts global positions to grid positions.
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    public List<Vector2> ConvertGlobPosToGridPos(List<GameObject> objects) {
        List<Vector2> gridPositions = new List<Vector2>();
        for (int i = 0; i < objects.Count; ++i) {
            Vector2 gridPos = GetGridIndex(objects[i].transform.position);
            gridPositions.Add(gridPos);
        }
        return gridPositions;
    }

    public List<Vector2> GetPassingThroughPieces(List<Vector2> gridPosition, Vector2 targettedGridPos) {
        List<Vector2> piecesGridPositions = new List<Vector2>();
        for (int i = 0; i < gridPosition.Count; ++i) {
            List<Vector2> movableRange = GetPossibleMoves(gridPosition[i], false);
            for (int j = 0; j < movableRange.Count; ++j) {
                if (movableRange[j] != targettedGridPos)
                    continue;
                piecesGridPositions.Add(gridPosition[i]);
                break;
            }
        }
        return piecesGridPositions;
    }

    /// <summary>
    /// Get in-between blocks which are present in from position to destination position.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Vector2> GetInBetweenBlocks(Vector2 from, Vector2 to) {
        ShuffleMinMaxValue(ref from, ref to);
        List<Vector2> possibleBlocks = new List<Vector2>();
        string patternType = GetMovingDirectionType(from, to);

        if (patternType == "Vertical")
            FromTillVerticalBlocks(ref possibleBlocks, from, to);

        else if (patternType == "Horizontal")
            FromTillHorizontalBlocks(ref possibleBlocks, from, to);

        else if (patternType == "Diagonal")
            FromTillDiagonalBlocks(ref possibleBlocks, from, to);

        return possibleBlocks;
    }

    public void FromTillHorizontalBlocks(ref List<Vector2> possibleBlocks, Vector2 from, Vector2 to) {
        int coundIndex = (int)from.y - (int)to.y;
        for (int i = 0; i < coundIndex; ++i) {
            Vector2 tempPos = new Vector2(to.x, to.y + i);
            possibleBlocks.Add(tempPos);
        }
    }

    public void FromTillVerticalBlocks(ref List<Vector2> possibleBlocks, Vector2 from, Vector2 to) {
        int countIndex = (int)from.x - (int)to.x;
        for (int i = 0; i < countIndex; ++i) {
            Vector2 tempPos = new Vector2(to.x + i, to.y);
            possibleBlocks.Add(tempPos);
        }
    }

    public void FromTillDiagonalBlocks(ref List<Vector2> possibleBlocks, Vector2 from, Vector2 to) {
        int xShifter = (from.x < to.x) ? -1 : 1;
        int yShifter = (from.y < to.y) ? -1 : 1;
        int coundIndex = Mathf.Abs((int)from.x - (int)to.x);
        for (int i = 0; i < coundIndex; ++i) {
            Vector2 tempPos = new Vector2(to.x + (i * xShifter), to.y + (i * yShifter));
            possibleBlocks.Add(tempPos);
        }
    }

    private string GetMovingDirectionType(Vector2 from, Vector2 to) {
        if (from.x == to.x)
            return "Vertical";
        if (from.y == to.y)
            return "Horizontal";
        return "Diagonal";
    }

    private void ShuffleMinMaxValue(ref Vector2 from, ref Vector2 to) {
        int fromKey = GetIndexKey(from);
        int toKey = GetIndexKey(to);
        if (fromKey > toKey)
            return;

        Vector2 temp = from;
        from = to;
        to = temp;
    }

    public List<Config.KillingPrioriety> GetInDangerPieces(string myTag) {
        myTag = RevertTag(myTag);
        List<Vector2> movablePieces = GetMovablePieceGridIndex(myTag, false);
        List<Config.KillingPrioriety> piecesWhichCanKill = GetPiecesWhichCanKill(movablePieces);
        return piecesWhichCanKill;
    }

    public List<Config.KillingPrioriety> GetPiecesWhichCanKill(List<Vector2> movablePieces) {
        List<Config.KillingPrioriety> killingPieces = new List<Config.KillingPrioriety>();
        for (int i = 0; i < movablePieces.Count; ++i) {
            List<Vector2> movableRange = GetPossibleMoves(movablePieces[i], false);
            for (int j = 0; j < movableRange.Count; ++j) {
                Config.KillingPrioriety hunterPiece = new Config.KillingPrioriety();
                GameObject obj = GetObjectOnGrid(movableRange[j]);
                if (obj == null)
                    continue;

                hunterPiece.prioriety = GetPiecePrioriety(obj);
                SetKillingPriorietyVars(ref hunterPiece, movablePieces[i], movableRange[j]);
                killingPieces.Add(hunterPiece);
                //if (CheckAndSetSupremePrioriety(ref killingPieces, hunterPiece))
                //    goto outerContinue;
            }
        }
        //outerContinue:
            return killingPieces;
    }

    public void SetKillingPriorietyVars(ref Config.KillingPrioriety hunterPiece, Vector2 gridPos, Vector2 targetPos) {
        hunterPiece.pieceGridPos = gridPos;
        hunterPiece.targetGridPos = targetPos;
    }

    public bool CheckAndSetSupremePrioriety(ref List<Config.KillingPrioriety> killingPieces, Config.KillingPrioriety hunterPiece) {
        for (int i = 0; i < killingPieces.Count; ++i) {
            if (killingPieces[i].prioriety == Config.SUPREME_PRIORITY) {
                killingPieces.Clear();
                killingPieces.Add(hunterPiece);
                return true;
            }
        }
        return false;
    }

    public int GetPiecePrioriety(Vector2 targetGridPos) {
        GameObject obj = GetObjectOnGrid(targetGridPos);
        return GetPiecePrioriety(obj);
    }

    public int GetPiecePrioriety(GameObject targetGameObj) {
        string typeStr = targetGameObj.GetComponent<PieceProperties>().typeStr;
        if (typeStr == Config.KING)
            return Config.SUPREME_PRIORITY;
        if (typeStr == Config.QUEEN)
            return Config.HIGH_PRIORITY;
        if (typeStr == Config.ROOK || typeStr == Config.BISHOP || typeStr == Config.KNIGHT)
            return Config.MEDIUM_PRIORITY;
        return Config.LOW_PRIORITY;
    }

    /// <summary>
    /// Reverts the tag of the team
    /// if player having white tag then returns black tag.
    /// same as for black tag.
    /// </summary>
    /// <param name="myTag"></param>
    /// <returns></returns>
    public string RevertTag(string myTag) {
        return (myTag == Config.WHITE_TAG) ? Config.BLACK_TAG : Config.WHITE_TAG;
    }

    private bool IsSupportingThePiece(Vector2 pieceGridPos, bool supporting) {
        if (!supporting)
            return supporting;
        return gameplayScene.aiKingCheckmateByPlayerPiece == pieceGridPos;
    }

    public bool IsSupportingthePiece(Vector2 targetPos, string myTag) {
        myTag = RevertTag(myTag);
        gameplayScene.aiKingCheckmateByPlayerPiece = targetPos;
        List<Vector2> movablePieces = GetMovablePieceGridIndex(myTag, true); // wrong code here correct it
        gameplayScene.ResetAIKingCheckmatePiece();
        return movablePieces.Count > 0;
    }

    public Vector2 GetSafeZoneToMove(List<Vector2> movableRange, string myTag) {
        myTag = RevertTag(myTag);
        List<Vector2> movablePieces = GetMovablePieceGridIndex(myTag, false);
        Vector2 move = Vector2.zero;
        
        for (int i = 0; i < movableRange.Count; ++i) {
            List<Vector2> piecesPassingThrough = GetPassingThroughPieces(movablePieces, movableRange[i]);
            if (piecesPassingThrough.Count > 0)
                continue;
            move = movableRange[i];
        }
        return move;
    }

    public void SortKillingPiecesBasedOnProriety(ref List<Config.KillingPrioriety> piecesKillingEnemies)
    {
        for (int i = 0; i < piecesKillingEnemies.Count; ++i)
        {
            for (int j = i + 1; j < piecesKillingEnemies.Count; ++j)
            {
                Config.KillingPrioriety killingPieceA = piecesKillingEnemies[i];
                Config.KillingPrioriety killingPieceB = piecesKillingEnemies[j];
                int aPrioriety = GetPiecePrioriety(killingPieceA.pieceGridPos);
                int bPrioriety = GetPiecePrioriety(killingPieceB.pieceGridPos);
                bool isAPriorietyLesser = aPrioriety < bPrioriety;
                if (isAPriorietyLesser)
                    continue;
                piecesKillingEnemies[i] = killingPieceB;
                piecesKillingEnemies[i + 1] = killingPieceA;
            }
        }
    }

    private bool IsKillingPriorietyContainsTargetPos(List<Config.KillingPrioriety> priorietyPieces, Vector2 targetGridPos) {
        for (int i = 0; i < priorietyPieces.Count; ++i) {
            Config.KillingPrioriety killingPrioriety = priorietyPieces[i];
            if (killingPrioriety.targetGridPos == targetGridPos)
                return true;
        }
        return false;
    }

    public void UpdateWithBlockingPieces(ref Config.KillingPrioriety killingPrioriety, List<Vector2> piecesChallangingTheKing, string tag) {
        Vector2 kingPos = GetPieceGridPosByString(Config.KING, tag);
        List<GameObject> availablePieces = GetAvailablePiecesWithoutKing(tag);
        for (int j = 0; j < piecesChallangingTheKing.Count; ++j) {
            for (int i = 0; i < availablePieces.Count; ++i) {
                List<Vector2> inbetweenBlocks = GetInBetweenBlocks(kingPos, piecesChallangingTheKing[j]);

                Vector2 gridIndex = GetGridIndex(availablePieces[i].transform.position);
                List<Vector2> movingAreas = GetPossibleMoves(gridIndex, false);
                if (movingAreas.Count == 0)
                    continue;
                for (int k = 0; k < inbetweenBlocks.Count; ++k) {
                    if (!movingAreas.Contains(inbetweenBlocks[k]))
                        continue;
                    killingPrioriety.pieceGridPos = gridIndex;
                    killingPrioriety.targetGridPos = inbetweenBlocks[k];
                }
            }
        }
    }
}
