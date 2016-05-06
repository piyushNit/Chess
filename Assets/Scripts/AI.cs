using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AI : MonoBehaviour {
    GameManager gameManager;
    [HideInInspector] public string myTag;
    [SerializeField] private float pieceWaitTime = 0.2f;

    Vector2 playerPiece;

    struct KillingPrioriety {
        public int prioriety;
        public Vector2 pieceGridPos;
        public Vector2 targetGridPos;
    }

    GameplayScene gameplayScene;

    void Start() {
        InitMemberVars();
        //PlayAiTurn();
    }
    void Update() {
        if (Input.GetKeyUp(KeyCode.Space))
            PlayAiTurn();
    }
/*    void OnEnable() {
        InitMemberVars();
        PlayAiTurn();
    }
*/
    private void InitMemberVars() {
        if(gameManager == null)
            gameManager = GameObject.FindObjectOfType<GameManager>();
        myTag = Config.BLACK_TAG;
        if(gameplayScene == null)
            gameplayScene = GameObject.FindObjectOfType<GameplayScene>();
    }

    public void PlayAiTurn() {
        if (gameplayScene.gameComplete)
            return;
        //if (IsCheckMate())
        //    //DefendTheKing();
        //else
            MoveNormalPieces();
    }

    private bool IsCheckMate() {
        Vector2 kingPos = gameManager.GetPieceGridPosByString(Config.KING, myTag);

        List<Vector2> movableRange = gameManager.GetPossibleMoves(playerPiece);
        for (int i = 0; i < movableRange.Count; ++i) {
            if (movableRange[i] == kingPos)
                return true;
        }
        return false;
    }

    private void DefendTheKing() {
        Vector2 kingPos = gameManager.GetPieceGridPosByString(Config.KING, myTag);
        List<Vector2> inbetweenBlocks = gameManager.GetInBetweenBlocks(kingPos, playerPiece);
        Vector2 targetedGridPos = Vector2.zero;
        for (int i = 0; i < inbetweenBlocks.Count; ++i) {
           // targetedGridPos = GetPiecesWhichCanPassThrough(inbetweenBlocks[i]);
            if (targetedGridPos != Vector2.zero)
                goto MoveThePiece;
        }
        return;
        MoveThePiece:
        DecidePieceToMove(playerPiece, targetedGridPos);
    }

    public void SetPlayerPiece(Vector2 gridPos) {
        playerPiece = gridPos;
    }

    private void MoveNormalPieces() {
        List<Vector2> movablePieces = new List<Vector2>();
        movablePieces = gameManager.GetMovableObjects(myTag);
        List<KillingPrioriety> killingPieces = GetPiecesWhichCanKill(movablePieces);
        UpdateAIMove(killingPieces);
        if (CheckForGameOver(killingPieces))
            gameplayScene.GameOver("Ai");
    }

    List<KillingPrioriety> GetPiecesWhichCanKill(List<Vector2> movablePieces) {
        List<KillingPrioriety> killingPieces = new List<KillingPrioriety>();
        for (int i = 0; i < movablePieces.Count; ++i) {
            List<Vector2> movableRange = gameManager.GetPossibleMoves(movablePieces[i]);
            for (int j = 0; j < movableRange.Count; ++j) {
                KillingPrioriety hunterPiece = new KillingPrioriety();
                if (gameManager.GetObjectOnGrid(movableRange[j]) == null)
                    hunterPiece.prioriety = Config.NO_PRIORITY;
                else
                    hunterPiece.prioriety = GetPiecePrioriety(movableRange[j]);
                SetKillingPriorietyVars(ref hunterPiece, movablePieces[i], movableRange[j]);
                killingPieces.Add(hunterPiece);
                if (CheckAndSetSupremePrioriety(ref killingPieces, hunterPiece))
                    goto outerContinue;
            }
        }
        outerContinue:
        return killingPieces;
    }

    private void SetKillingPriorietyVars(ref KillingPrioriety hunterPiece, Vector2 gridPos, Vector2 targetPos) {
        hunterPiece.pieceGridPos = gridPos;
        hunterPiece.targetGridPos = targetPos;
    }

    private bool CheckAndSetSupremePrioriety(ref List<KillingPrioriety> killingPieces, KillingPrioriety hunterPiece) {
        for(int i = 0; i < killingPieces.Count; ++i) {
            if (killingPieces[i].prioriety == Config.SUPREME_PRIORITY) {
                killingPieces.Clear();
                killingPieces.Add(hunterPiece);
                return true;
            }
        }
        return false;
    }

    private int GetPiecePrioriety(Vector2 targetGridPos) {
        GameObject obj = gameManager.GetObjectOnGrid(targetGridPos);
        string typeStr = obj.GetComponent<PieceProperties>().typeStr;
        if (typeStr == Config.KING)
            return Config.SUPREME_PRIORITY;
        if (typeStr == Config.QUEEN)
            return Config.HIGH_PRIORITY;
        if (typeStr == Config.ROOK || typeStr == Config.BISHOP || typeStr == Config.KNIGHT)
            return Config.MEDIUM_PRIORITY;
        return Config.LOW_PRIORITY;
    }

    private void KillTheKing(KillingPrioriety value) {
        int key = gameManager.GetIndexKey(value.pieceGridPos);
        GameObject obj = gameManager.GetObjectOnGrid(value.pieceGridPos);
        Vector3 targetPosition = gameManager.GetGlobalCoords(value.targetGridPos);
        gameManager.SetPieceOnPosition(key, obj);

        iTween.MoveTo(obj, targetPosition, gameManager.pieceMoveSpeed);
    }

    private bool CheckForGameOver(List<KillingPrioriety> killingPieces) {
        for (int index = 0; index < killingPieces.Count; ++index) {
            if (killingPieces[index].prioriety == Config.SUPREME_PRIORITY)
                return true;
        }
        return false;
    }

    private void UpdateAIMove(List<KillingPrioriety> killingPieces) {
        if (IsQueenCanBeKilled(killingPieces[0])){
            gameplayScene.UpdatePlayerMove(killingPieces[0].pieceGridPos, killingPieces[0].targetGridPos);
            return;
        }
        DecidePieceToMove(killingPieces);
    }

    private bool IsQueenCanBeKilled(KillingPrioriety hunter) {
        if (hunter.prioriety == Config.HIGH_PRIORITY)
            return true;
        return false;
    }

    private void DecidePieceToMove(List<KillingPrioriety> killingPieces) {
        KillingPrioriety aiMovingPiece = GetAIMovingPiece(killingPieces);
        gameplayScene.SelectBlock(aiMovingPiece.pieceGridPos);
        StartCoroutine(TakeNextStep(aiMovingPiece.targetGridPos));
    }

    private void DecidePieceToMove(Vector2 playerGridPos, Vector2 targetGridPos) {
        gameplayScene.SelectBlock(playerGridPos);
        StartCoroutine(TakeNextStep(targetGridPos));
    }

    IEnumerator TakeNextStep(Vector2 targetPosition) {
        yield return new WaitForSeconds(pieceWaitTime);
        gameplayScene.SelectBlock(targetPosition);
    }

    private KillingPrioriety GetAIMovingPiece(List<KillingPrioriety> killingPieces) {
        KillingPrioriety movingPiece = new KillingPrioriety();
        int priorietyPiece = Config.NO_PRIORITY;
        for (int i = 0; i < killingPieces.Count; ++i) {
            if (priorietyPiece < killingPieces[i].prioriety)
                movingPiece = killingPieces[i];
        }
        bool pieceIsNull = (movingPiece.pieceGridPos == Vector2.zero);
        bool targetisNull = (movingPiece.targetGridPos == Vector2.zero);
        if(!pieceIsNull && !targetisNull)
            return movingPiece;
        int randomNum = Random.Range(0, killingPieces.Count);
        return killingPieces[randomNum];
    }

    /// <summary>
    /// Return Objects which can pass or move through given block grid position.
    /// </summary>
    /// <param name="gridPos"></param>
    //private Vector2 GetPiecesWhichCanPassThrough(Vector2 gridPos) {
    //    List<GameObject> availablePieces = gameManager.GetAvailablePieces(myTag);
    //    List<Vector2> availablePiecesGridPos = gameManager.ConvertGlobPosToGridPos(availablePieces);
    //    List<Vector2> picesWhichCanPassThrough = gameManager.CanPassThroughPieces(availablePiecesGridPos, gridPos);
    //    List<KillingPrioriety> killingPieces = ConvertGridPosToKillingPrioriety(picesWhichCanPassThrough, gridPos);
    //    return GetLowestPriorietyPiece(killingPieces);
    //}

    private Vector2 GetLowestPriorietyPiece(List<KillingPrioriety> killingPriorietyPieces) {
        int kilingPrioerietyPiece = 1;
        Vector2 killingPiece = Vector2.zero;
        for (int i = 0; i < killingPriorietyPieces.Count; ++i) {
            if (killingPriorietyPieces[i].prioriety < kilingPrioerietyPiece)
                continue;
            killingPiece = killingPriorietyPieces[i].pieceGridPos;
        }
        return killingPiece;
    }

    private List<KillingPrioriety> ConvertGridPosToKillingPrioriety(List<Vector2> gridPos, Vector2 targettedPos) {
        List<KillingPrioriety> piecesWithKillingProioriety = new List<KillingPrioriety>();
        for (int i = 0; i < gridPos.Count; ++i) {
            KillingPrioriety killingPrioriety = new KillingPrioriety();
            killingPrioriety.pieceGridPos = gridPos[i];
            killingPrioriety.targetGridPos = targettedPos;
            killingPrioriety.prioriety = GetPiecePrioriety(gridPos[i]);
            piecesWithKillingProioriety.Add(killingPrioriety);
        }
        return piecesWithKillingProioriety;
    }
}
