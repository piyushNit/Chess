﻿using UnityEngine;
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
    AI ai;
    ChessBoardProperties chessBoardProperties;
    private bool aiTurn = false;
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
                if (!IsValidTeamPeace(hit.point))
                    return;
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
        //Utils.DrawTotalGrid(gameManager.gameBoard);
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
        possibleMoves = gameManager.GetPossibleMoves(gridIndex);
        ToggleHighlightEnemyPieces(possibleMoves, true);
        PlayerCanMovePositions(possibleMoves);
    }

    private void ToggleHighlightEnemyPieces(List<Vector2> possibleMoves, bool value) {
        if (possibleMoves.Count == 0)
            return;
        for (int i = 0; i < possibleMoves.Count; ++i) {
            GameObject firstObj = gameManager.GetObjectOnGrid(possibleMoves[i]);
            if (firstObj != null)
                if (value)
                    ToggleObjColor(firstObj, inDangerColor);
                else
                    ToggleObjColor(firstObj, Color.white);
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
        if (!possibleMoves.Contains(gridIndex)){
            ToggleHighlightEnemyPieces(possibleMoves, false);
            DeselectLastSelectedPiece();
            return;
        }
        UpdatePlayerMove(gridIndex);
        //if single mode
        ai.SetPlayerPiece(gridIndex);
        ToggleHighlightEnemyPieces(possibleMoves, false);
        DeselectLastSelectedPiece();
        PlayAITurn();
    }

    private void UpdatePlayerMove(Vector2 targetGridIndex) {
        if (gameManager.CanKillPiece(targetGridIndex))
            MoveKilledPiecesToTrash(targetGridIndex);
        int key = gameManager.GetIndexKey(targetGridIndex);
        UpdateStatesInGameManager(key, selectedPiece.gameObject);
        Vector3 newPosition = gameManager.GetGlobalCoords(targetGridIndex);

        float time = GetTotalMovingTime(targetGridIndex);
        audioSrc.Play();
        iTween.MoveTo(selectedPiece.gameObject, newPosition, time);
        UpdateHUD();
    }

    public void UpdatePlayerMove(Vector2 playerGridIndex, Vector2 targetGridIndex) {
        if (gameManager.CanKillPiece(targetGridIndex))
            MoveKilledPiecesToTrash(targetGridIndex);
        int key = gameManager.GetIndexKey(targetGridIndex);
        GameObject playerPiece = gameManager.GetObjectOnGrid(playerGridIndex);
        UpdateStatesInGameManager(key, playerPiece);
        Vector3 newPosition = gameManager.GetGlobalCoords(targetGridIndex);

        float time = GetTotalMovingTime(targetGridIndex, playerPiece);
        audioSrc.Play();
        iTween.MoveTo(playerPiece.gameObject, newPosition, time);
        UpdateHUD();
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
        aiTurn = !aiTurn;
        if (aiTurn)
            ai.PlayAiTurn();
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
}