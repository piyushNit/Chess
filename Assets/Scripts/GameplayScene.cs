using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameplayScene : MonoBehaviour {
	RaycastHit hit;
	GameManager gameManager;
    [SerializeField] private GameObject selectionPlane;
    [SerializeField] private GameObject particleEfect;
    [SerializeField] private Color inDangerColor = Color.red;
    [HideInInspector] public bool canPlayTheGame = true;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text bestMoveText;
    [SerializeField] private Text moveText;

    private int score = 0;
    private int moves = 0;
    public string myTag = Config.WHITE_TAG;
    private Transform selectedPiece;
    private AudioSource audioSrc;
    GameObject[] selectedBlocks;
    GameObject planeHolder;
    List<Vector2> possibleMoves;
    private bool _gameComplete = false;
    public bool gameComplete { get { return _gameComplete; }}
    ChessBoardProperties chessBoardProperties;
    public bool aiTurn = false;
    public bool isAI = true;

    Stack<Config.PieceAction> undoQueue;
    private bool isPlaying = false;

    //AI related code
    AI ai;
    public Vector2 aiKingCheckmateByPlayerPiece = Vector2.zero;

    void Start(){
        InitMemberVars();
        UpdateSelectionPlane();
    }

    void InitMemberVars() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        planeHolder = new GameObject("Plane Holder");
        selectedBlocks = new GameObject[27];
        possibleMoves = new List<Vector2>();
        for (int i = 0; i < selectedBlocks.Length; ++i) {
            GameObject obj = Instantiate(selectionPlane, selectionPlane.transform.position, selectionPlane.transform.rotation) as GameObject;
            obj.transform.parent = planeHolder.transform;
            selectedBlocks[i] = obj;
        }
        audioSrc = this.GetComponent<AudioSource>();
        //temp initilize here
        ai = GameObject.FindObjectOfType<AI>();
        chessBoardProperties = GameObject.FindObjectOfType<ChessBoardProperties>();
        undoQueue = new Stack<Config.PieceAction>();
        InitHUD();
    }

    void InitHUD() {
        scoreText.text = "Score: " + score;
        moveText.text = "Moves: " + moves;
        InitBestMove();
    }

    void InitBestMove() {
        int bestMove = PlayerPrefs.GetInt(Keys.KEY_BEST_MOVE);
        bestMoveText.text = "Best Moves: " + bestMove;
    }


	void Update(){
        if (!canPlayTheGame || gameComplete)
            return;
        if (aiTurn)
            return;
		#if(UNITY_EDITOR)
		if (Input.GetButtonDown ("Fire1")) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * 500f);
			if(Physics.Raycast(ray, out hit, 500f)) {
				if(!IsTouchOnBoard(hit.point))
					return;
                //if (!IsValidTeamPeace(hit.point))
                //    return;
                SelectBlock(gameManager.GetGridIndex(hit.point));
			}
		}
		#endif
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if(Physics.Raycast(ray, out hit, 500f)) {
                if (!IsTouchOnBoard(hit.point))
                    return;
                if (!IsValidTeamPeace(hit.point))
                    return;
                SelectBlock(gameManager.GetGridIndex(hit.point));
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private bool IsValidTeamPeace(Vector3 touchPos) {
        Vector2 gridPos = gameManager.GetGridIndex(touchPos);
        GameObject pieceObj = gameManager.GetObjectOnGrid(gridPos);
        if (pieceObj == null)
            return true;
        if(selectedPiece != null && !aiTurn)
            return true;
        return (myTag == pieceObj.tag);
    }

	private bool IsTouchOnBoard(Vector3 touchPos){
		return (touchPos.x > ChessBoardManager.ChessBoardM.StartXY.x && touchPos.x < ChessBoardManager.ChessBoardM.EndXY.x &&
			touchPos.z > ChessBoardManager.ChessBoardM.StartXY.y && touchPos.z < ChessBoardManager.ChessBoardM.EndXY.y);
	}

    void UpdateSelectionPlane() {
        selectionPlane.SetActive(false);
    }

    public void SelectBlock(Vector2 gridIndex) {
        //Debug.Log("x: " + gridIndex.x + " y: " + gridIndex.y);
        UpdateSeclectedBlock(gridIndex);
        if (selectedPiece != null)
            UpdatePossibleMoves(gridIndex);
        else
            SelectLastSelectedPiece(gridIndex);
    }

    private void UpdateSeclectedBlock(Vector2 gridIndex) {
        if (selectedPiece != null)
           return; // show not movable error sprite here
        ToggleSelectionPlane(gridIndex);
    }

    private void SelectLastSelectedPiece(Vector2 gridIndex) {
        int blockIndex = gameManager.GetIndexKey(gridIndex);
        Transform pieceTrans = gameManager.GetSelectablePieceTransform(blockIndex);
        if (pieceTrans == null)
            return;
        selectedPiece = pieceTrans;
        selectedPiece.position += selectedPiece.up * Config.SELECT_UP_MOVE;
        possibleMoves = gameManager.GetPossibleMoves(gridIndex, false);
        ToggleHighlightEnemyPieces(possibleMoves, true);
        PlayerCanMovePositions(possibleMoves);
        isPlaying = true;
    }

    private void ToggleHighlightEnemyPieces(List<Vector2> possibleMoves, bool value) {
        if (possibleMoves.Count == 0)
            return;
        for (int i = 0; i < possibleMoves.Count; ++i) {
            GameObject obj = gameManager.GetObjectOnGrid(possibleMoves[i]);
            if (obj != null)
                if (value)
                    ToggleObjColor(obj, inDangerColor);
                else
                    ToggleObjColor(obj, Color.white);
        }
    }

    private void ToggleObjColor(GameObject obj, Color color) {
        obj.GetComponentInChildren<Renderer>().material.color = color;
    }

    private void PlayerCanMovePositions(List<Vector2> possibleMoves) {
        for (int i = 0; i < possibleMoves.Count; ++i) {
            Vector3 newPosition = gameManager.GetGlobalCoords(possibleMoves[i]);
            newPosition.y = selectionPlane.transform.position.y;
            EnableSelectedTile(newPosition, i);
        }
    }

    private void DeselectLastSelectedPiece() {
        DisableSlectedTile();
        if (selectedPiece == null)
            return;
        selectedPiece.position -= selectedPiece.up * Config.SELECT_UP_MOVE;
        selectedPiece = null;
        selectionPlane.SetActive(false);
    }

    private void EnableSelectedTile(Vector3 position, int index) {
        GameObject obj = selectedBlocks[index];
        obj.SetActive(true);
        obj.transform.position = position;
    }

    private void DisableSlectedTile() {
        for (int i = 0; i < selectedBlocks.Length; ++i) {
            selectedBlocks[i].SetActive(false);
        }
    }

    private void ToggleSelectionPlane(Vector2 gridIndex) {
        Vector3 newPosition = gameManager.GetGlobalCoords(gridIndex);
        Vector3 pos1 = selectionPlane.transform.position;
        selectionPlane.transform.position = new Vector3(newPosition.x, selectionPlane.transform.position.y, newPosition.z);
        if (pos1 == selectionPlane.transform.position)
            selectionPlane.SetActive(!selectionPlane.activeSelf);
        else
            selectionPlane.SetActive(true);
    }

    private void TogglePieceSelection(Vector2 gridIndex) {
        if (selectedPiece == null)
            return;
        Vector3 newPosition = gameManager.GetGlobalCoords(gridIndex);
        Vector3 pos1 = new Vector3(newPosition.x, selectedPiece.transform.position.y, newPosition.z);
        if (pos1 != selectedPiece.transform.position)
            return;
        selectedPiece.position = gameManager.GetGlobalCoords(gridIndex);
        selectedPiece = null;
        DisableSlectedTile();
        selectionPlane.SetActive(false);
        ToggleHighlightEnemyPieces(possibleMoves, false);
    }

    private void UpdatePossibleMoves(Vector2 gridIndex) {
        TogglePieceSelection(gridIndex);
        if (selectionPlane == null)
            return;
        if (!possibleMoves.Contains(gridIndex)) {
            ToggleHighlightEnemyPieces(possibleMoves, false);
            DeselectLastSelectedPiece();
            return;
        }
        UpdatePlayerMove(gridIndex);
        //if single mode
        ai.SetPlayerPiece(gridIndex);
        ToggleHighlightEnemyPieces(possibleMoves, false);
        DeselectLastSelectedPiece();
        isPlaying = false;
    }

    private void UpdatePlayerMove(Vector2 targetGridIndex) {
        Vector2 playerPiecePos = gameManager.GetGridIndex(selectedPiece.transform.position);
        UpdateUndoQueue(playerPiecePos, targetGridIndex);
        if (gameManager.CanKillPiece(targetGridIndex))
            MoveKilledPiecesToTrash(targetGridIndex);
        UpdatePlayerAction(targetGridIndex);
    }

    private void UpdatePlayerAction(Vector2 targetGridIndex) {
        int key = gameManager.GetIndexKey(targetGridIndex);
        UpdateStatesInGameManager(key, selectedPiece.gameObject);

        float time = GetTotalMovingTime(targetGridIndex);
        StartCoroutine(MoveAnimation(targetGridIndex, selectedPiece.gameObject, time, false));
        UpdateHUD();
        UpdateMoves(selectedPiece.gameObject);
    }

    public void UpdatePlayerMove(Vector2 playerGridIndex, Vector2 targetGridIndex) {
        UpdateUndoQueue(playerGridIndex, targetGridIndex);
        if (gameManager.CanKillPiece(targetGridIndex))
            MoveKilledPiecesToTrash(targetGridIndex);
        int key = gameManager.GetIndexKey(targetGridIndex);
        GameObject playerPiece = gameManager.GetObjectOnGrid(playerGridIndex);
        UpdateMoves(playerPiece);
        UpdateStatesInGameManager(key, playerPiece);

        float time = GetTotalMovingTime(targetGridIndex, playerPiece);
        StartCoroutine(MoveAnimation(targetGridIndex, playerPiece, time, false));
        UpdateHUD();
    }

    public void UpdateMoves(GameObject obj) {
        PieceProperties pieceProp = obj.GetComponent<PieceProperties>();
        pieceProp.UpdateMoves();
    }

    IEnumerator MoveAnimation(Vector2 targetGridIndex, GameObject playerPiece, float time, bool isUndo) {
        if (!isUndo)
            PlayAITurn();
        audioSrc.Play();
        Vector3 newPosition = gameManager.GetGlobalCoords(targetGridIndex);
        iTween.MoveTo(playerPiece.gameObject, newPosition, time);
        yield return new WaitForSeconds(time + 0.5f);
        StartCoroutine(PlayAIItsTurn(time));
    }

    IEnumerator PlayAIItsTurn(float time) {
        yield return new WaitForSeconds(time + 0.5f);
        if(aiTurn)
            ai.PlayAiTurn();
    }

    private void UpdateHUD() {
        if (aiTurn)
            return;
        score++;
        scoreText.text = "Score: " + score;
        moves++;
        moveText.text = "Moves: " + moves;
    }

    private void PlayAITurn() {
        if (!isAI)
            return;
        aiTurn = !aiTurn;
            
    }

    private void UpdateStatesInGameManager(int key, GameObject playerPiece) {
        gameManager.UpdatePawnPosition(playerPiece);
        gameManager.SetPieceOnPosition(key, playerPiece);
    }

    private float GetTotalMovingTime(Vector2 gridIndex, GameObject playerPiece) {
        int totalBLocks = 1;
        Vector2 playerGrid = gameManager.GetGridIndex(playerPiece.transform.position);
        if (playerGrid.x == gridIndex.x)
            totalBLocks = (int)Mathf.Abs(playerGrid.y - gridIndex.y);
        else
            totalBLocks = (int)Mathf.Abs(playerGrid.x - gridIndex.x);
        float deltaTime = gameManager.pieceMoveSpeed / Config.BOARD_BLOCKS;
        return totalBLocks * deltaTime;
    }

    private float GetTotalMovingTime(Vector2 gridIndex) {
        int totalBLocks = 1;
        Vector2 playerGrid = gameManager.GetGridIndex(selectedPiece.transform.position);
        if (playerGrid.x == gridIndex.x)
            totalBLocks = (int)Mathf.Abs(playerGrid.y - gridIndex.y);
        else
            totalBLocks = (int)Mathf.Abs(playerGrid.x - gridIndex.x);
        float deltaTime = gameManager.pieceMoveSpeed / Config.BOARD_BLOCKS;
        return totalBLocks * deltaTime;
    }
    
    private void MoveKilledPiecesToTrash(Vector2 gridIndex) {
        GameObject obj = gameManager.GetObjectOnGrid(gridIndex);
        ToggleObjColor(obj, Color.white);
        Vector3 position = Vector3.zero;
        if (gameManager.IsWhiteTeamPiece(obj))
            position = chessBoardProperties.wPieceHolder.transform.position;
        else
            position = chessBoardProperties.bPieceHolder.transform.position;
        iTween.MoveTo(obj, position, gameManager.pieceMoveSpeed);
        //obj.SetActive(false);//add additional code here
        Vector3 existingPos = gameManager.GetGlobalCoords(gridIndex);
        gameManager.UpdatePieceProperties(gridIndex);
        PlayEndEffects(existingPos);
        if(IsThisKing(obj))
            GameOver("Player");
    }

    private bool IsThisKing(GameObject obj) {
        PieceProperties pieceProperties = obj.GetComponent<PieceProperties>();
        return (pieceProperties.typeStr == Config.KING);
    }

    private void PlayEndEffects(Vector3 position) {
        Instantiate(particleEfect, position, Quaternion.identity);
    }

    public void GameOver(string winnignTeam) {
        _gameComplete = true;
        Debug.Log("game over: " + winnignTeam + " has won the game");
        UpdateBestMoves();
    }
    void UpdateBestMoves() {
        int bestMove = PlayerPrefs.GetInt(Keys.KEY_BEST_MOVE);
        if (moves < bestMove || bestMove == 0)
            PlayerPrefs.SetInt(Keys.KEY_BEST_MOVE, moves);
    }

    public void ResetAIKingCheckmatePiece() {
        aiKingCheckmateByPlayerPiece = Vector2.zero;
    }

    public void UpdateUndoQueue(Vector2 currPos, Vector2 targetGridIndex) {
        Config.PieceAction pieceAction = new Config.PieceAction();
        pieceAction.pieceGridPos = currPos;
        pieceAction.pieceObj = gameManager.GetObjectOnGrid(currPos);

        if (gameManager.CanKillPiece(targetGridIndex)) {
            pieceAction.currGridPos = targetGridIndex;
            pieceAction.isKilling = true;
            pieceAction.killedPiece = gameManager.GetObjectOnGrid(targetGridIndex);
        }

        undoQueue.Push(pieceAction);
    }

    public void UndoTheStep() {
        if (undoQueue.Count <= 0 || isPlaying)
            return;
        Config.PieceAction pieceAction = undoQueue.Pop();
        int key = gameManager.GetIndexKey(pieceAction.pieceGridPos);
        UpdateStatesInGameManager(key, pieceAction.pieceObj);
        UndoPieceMoves(pieceAction.pieceObj);
        float time = GetTotalMovingTime(pieceAction.pieceGridPos, pieceAction.pieceObj);
        audioSrc.Play();
        StartCoroutine(MoveAnimation(pieceAction.pieceGridPos, pieceAction.pieceObj, time, true));

        if (!pieceAction.isKilling)
            return;
        time = GetTotalMovingTime(pieceAction.currGridPos, pieceAction.killedPiece);
        StartCoroutine(MoveAnimation(pieceAction.currGridPos, pieceAction.killedPiece, time, true));
        //write further code from here
    }

    private void UndoPieceMoves(GameObject obj) {
        PieceProperties pieceProp = obj.GetComponent<PieceProperties>();
        pieceProp.UndoMoves();
    }
}
