using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AI : MonoBehaviour {
    GameManager gameManager;
    [HideInInspector] public string myTag;
    [SerializeField] private float pieceWaitTime = 0.2f;

    Vector2 playerPiece;
    private List<Vector2> justMovablePieces;
    private List<Config.KillingPrioriety> killingPieces;
    private List<Config.KillingPrioriety> piecesInDanger;

    GameplayScene gameplayScene;
    private Vector2 kingPos;

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
        piecesInDanger = new List<Config.KillingPrioriety>();
    }

    public void PlayAiTurn() {
        if (gameplayScene.gameComplete)
            return;
        AnalyzeChessBoard();
        if (IsCheckMate())
            CheckMateAnalysis();
        else
            MoveNormalPieces();
        ResetVariables();
    }

    private void AnalyzeChessBoard() {
        justMovablePieces = gameManager.GetMovablePieceGridIndex(myTag, false);
        killingPieces = gameManager.GetPiecesWhichCanKill(justMovablePieces);
        piecesInDanger = gameManager.GetInDangerPieces(myTag);
        UpdateKingPos();
    }

    private void AnalyzePieceToPlay() {
    }

    private void ResetVariables() {
        justMovablePieces.Clear();
        killingPieces.Clear();
        piecesInDanger.Clear();
    }

    private bool IsCheckMate() {
        return IsContainingKillingPiece(piecesInDanger, kingPos);
    }

    private bool IsContainingKillingPiece(Config.KillingPrioriety hunterPiece, Vector2 gridPos) {
        Config.KillingPrioriety killingPriority = hunterPiece;
        if (killingPriority.targetGridPos == gridPos)
            return true;
        return false;
    }

    private bool IsContainingKillingPiece(List<Config.KillingPrioriety> hunterPiece, Vector2 gridPos) {
        for (int i = 0; i < hunterPiece.Count; ++i) {
            bool contains = IsContainingKillingPiece(hunterPiece[i], gridPos);
            if (contains)
                return true;
        }
        return false;
    }

    private void UpdateKingPos() {
        kingPos = gameManager.GetPieceGridPosByString(Config.KING, myTag);
    }

    //private void CheckMateAnalysis() {
    //    //List<Vector2> piecesChallangingTheKing = new List<Vector2>();
    //    //piecesChallangingTheKing = FindPiecesChallingKing();
    //    //bool isSinglePieceChallanging = piecesChallangingTheKing.Count == 1;
    //    //if (isSinglePieceChallanging)
    //    //    UpdateForSingleStrategy(piecesChallangingTheKing);
    //    //else

    //    DefendTheKing();
    //}

    /*private void UpdateForSingleStrategy(List<Vector2> piecesChallangingTheKing) {
        bool isNoRange = IsTargettedPieceHavingNoRange(piecesChallangingTheKing);
        bool isPerfectStrategy = isNoRange ? IsPerfectStrategy(piecesChallangingTheKing[0]) : false;
        if (isPerfectStrategy)
            AnalyzeToDefendPerfectStrategy(piecesChallangingTheKing[0]);
        else {
            bool canKillByOther = IsContainingKillingPiece(killingPieces, piecesChallangingTheKing[0]);
            if (canKillByOther)
                KillThePiece(piecesChallangingTheKing[0]);
            else {
                bool isMovedToSafeZone = MoveToSafeZone();
                if (!isMovedToSafeZone) {
                    bool defendKing = DefendTheKing();
                    if (!defendKing)
                        CommitLose();
                }
            }
        }
    }
    */

    private void AnalyzeToDefendPerfectStrategy(Vector2 targetPiece) {
        bool isKilledByOther = CanKillByOtherPieces(targetPiece);
        if (isKilledByOther)
            return;
        //find king moving areas
        // find can kill the piece
        // if there is no option then commit lose
        CommitLose();
    }

    private bool MoveToSafeZone() {
        List<Vector2> kingMovingAreas = gameManager.GetKingMoves(kingPos, false);
        if (kingMovingAreas.Count <= 0) {
            CommitLose();
            return true;
        }
        Vector2 safeZoneToMove = gameManager.GetSafeZoneToMove(kingMovingAreas, myTag);
        if (safeZoneToMove != Vector2.zero) {
            gameplayScene.UpdatePlayerMove(kingPos, safeZoneToMove);
            return true;
        }
        return false;
    }

    private bool CanKillByOtherPieces(Vector2 targetPiece) {
        for (int i = 0; i < killingPieces.Count; ++i) {
            if (IsContainingKillingPiece(killingPieces, targetPiece)) {
                gameplayScene.UpdatePlayerMove(killingPieces[i].pieceGridPos, targetPiece);
                return true;
            }
        }
        return false;
    }

    private void KillThePiece(Vector2 targetPiece) {
        gameplayScene.UpdatePlayerMove(killingPieces[0].pieceGridPos, killingPieces[0].targetGridPos);
    }

    private bool IsPerfectStrategy(Vector2 challangingPiece) {
        bool isSupporting = gameManager.IsSupportingthePiece(challangingPiece, myTag);
        return isSupporting;
    }

    private void CommitLose() {
        gameplayScene.GameOver("AI");
    }

    private List<Vector2> FindPiecesChallingKing() {
        List<Vector2> piecesChallangingTheKing = new List<Vector2>();
        for (int i = 0; i < piecesInDanger.Count; ++i) {
            Config.KillingPrioriety killingPrioriety = piecesInDanger[i];
            if (IsContainingKillingPiece(piecesInDanger[i], kingPos))
                piecesChallangingTheKing.Add(killingPrioriety.pieceGridPos);
        }
        return piecesChallangingTheKing;
    }

    private bool IsTargettedPieceHavingNoRange(List<Vector2> piecesChallangingTheKing) {
        int x = Mathf.Abs((int)piecesChallangingTheKing[0].x - (int)kingPos.x);
        int y = Mathf.Abs((int)piecesChallangingTheKing[0].y - (int)kingPos.y);
        return (x == 1 || y == 1) ? true : false;
    }

    private void CheckMateAnalysis() {
        List<Vector2> piecesChallangingTheKing = new List<Vector2>();
        piecesChallangingTheKing = FindPiecesChallingKing();
        //kill the piece by lowest prioriety
        bool isKilledByOtherPiece = KillThePieceWithLowestProriety(piecesChallangingTheKing);
        if (isKilledByOtherPiece)
            return;
        bool isBlockedTheWayToSaveTheKing = BlockTheWayToSaveTheKing(piecesChallangingTheKing);
        if (isBlockedTheWayToSaveTheKing)
            return;
        bool isMovedToSafeZone = MoveToSafeZone();
        if (isMovedToSafeZone)
            return;
        CommitLose();
        //if not kill then find the save zone to move the king
        // if not finding safe zone then commit lose
    }

    private bool KillThePieceWithLowestProriety(List<Vector2> piecesChllangingTheking) {
        List<Config.KillingPrioriety> aiPiecesKillingProrities = FindPiecesKilledByLowestPrioriety(piecesChllangingTheking);
        if (aiPiecesKillingProrities.Count == 0)
            return false;
        Config.KillingPrioriety killingPrioriety = aiPiecesKillingProrities[0];
        UpdateAIMoveingPieceWithTarget(killingPrioriety.pieceGridPos, killingPrioriety.targetGridPos);
        return true;
    }

    private bool BlockTheWayToSaveTheKing(List<Vector2> piecesChallangingTheKing) {
        Config.KillingPrioriety killingPrioriety = new Config.KillingPrioriety();
        killingPrioriety.pieceGridPos = Vector2.zero;
        gameManager.UpdateWithBlockingPieces(ref killingPrioriety, piecesChallangingTheKing, myTag);

        if (killingPrioriety.pieceGridPos == Vector2.zero)
            return false;
        gameplayScene.UpdatePlayerMove(killingPrioriety.pieceGridPos, killingPrioriety.targetGridPos);
        return true;
    }

    private List<Config.KillingPrioriety> FindPiecesKilledByLowestPrioriety(List<Vector2> piecesChallangingTheKing) {
        List<Config.KillingPrioriety> possibleKillingProrities = new List<Config.KillingPrioriety>();
        for (int i = 0; i < killingPieces.Count; ++i) {
            for (int j = 0; j < piecesChallangingTheKing.Count; ++j) {
                bool canBeKilled = IsContainingKillingPiece(killingPieces[i], piecesChallangingTheKing[j]);
                if (canBeKilled)
                    possibleKillingProrities.Add(killingPieces[i]);
            }
        }
        gameManager.SortKillingPiecesBasedOnProriety(ref possibleKillingProrities);
        return possibleKillingProrities;
    }

    public void SetPlayerPiece(Vector2 gridPos) {
        playerPiece = gridPos;
    }

    private void MoveNormalPieces() {
        if(killingPieces.Count > 0)
            UpdateAIMove(killingPieces);
        else
            UpdateAIMovingPiece();
        if (CheckForGameOver(killingPieces))
            gameplayScene.GameOver("Ai");
    }

    private void UpdateAIMovingPiece() {
        if (justMovablePieces.Count <= 0)
            return;
        int randomIndex = Random.Range(0, justMovablePieces.Count);
        Vector2 selMovingPiece = justMovablePieces[randomIndex];
        List<Vector2> movingRange = gameManager.GetPossibleMoves(selMovingPiece, false);
        int moveIndex = Random.Range(0, movingRange.Count);
        //check whether anyone is targetting the desired position or not
        //if yes then call the same function - function recurssion
        UpdateAIMoveingPieceWithTarget(selMovingPiece, movingRange[moveIndex]);
    }

    private void UpdateAIMoveingPieceWithTarget(Vector2 piece, Vector2 targetPiece) {
        gameplayScene.SelectBlock(piece);
        StartCoroutine(TakeNextStep(targetPiece));
    }

    private void KillTheKing(Config.KillingPrioriety value) {
        int key = gameManager.GetIndexKey(value.pieceGridPos);
        GameObject obj = gameManager.GetObjectOnGrid(value.pieceGridPos);
        Vector3 targetPosition = gameManager.GetGlobalCoords(value.targetGridPos);
        gameManager.SetPieceOnPosition(key, obj);

        iTween.MoveTo(obj, targetPosition, gameManager.pieceMoveSpeed);
    }

    private bool CheckForGameOver(List<Config.KillingPrioriety> killingPieces) {
        for (int index = 0; index < killingPieces.Count; ++index) {
            if (killingPieces[index].prioriety == Config.SUPREME_PRIORITY)
                return true;
        }
        return false;
    }

    private void UpdateAIMove(List<Config.KillingPrioriety> killingPieces) {
        if (IsQueenCanBeKilled(killingPieces[0])){
            gameplayScene.UpdatePlayerMove(killingPieces[0].pieceGridPos, killingPieces[0].targetGridPos);
            return;
        }
        DecidePieceToMove(killingPieces);
    }

    private bool IsQueenCanBeKilled(Config.KillingPrioriety hunter) {
        if (hunter.prioriety == Config.HIGH_PRIORITY)
            return true;
        return false;
    }

    private void DecidePieceToMove(List<Config.KillingPrioriety> killingPieces) {
        Config.KillingPrioriety aiMovingPiece = GetAIMovingPiece(killingPieces);
        if (!aiMovingPiece.isKilling) {
            UpdateAIMovingPiece();
            return;
        }
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

    private Config.KillingPrioriety GetAIMovingPiece(List<Config.KillingPrioriety> killingPieces) {
        Config.KillingPrioriety movingPiece = new Config.KillingPrioriety();
        int priorietyPiece = Config.NO_PRIORITY;
        for (int i = 0; i < killingPieces.Count; ++i) {
            if (priorietyPiece < killingPieces[i].prioriety) {
                movingPiece = killingPieces[i];
                priorietyPiece = movingPiece.prioriety;
            }
        }
        bool pieceIsNull = (movingPiece.pieceGridPos == Vector2.zero);
        bool targetisNull = (movingPiece.targetGridPos == Vector2.zero);
        if (!pieceIsNull && !targetisNull)
            return movingPiece;
        int randomNum = Random.Range(0, killingPieces.Count);

        Vector2 movingPos = killingPieces[randomNum].targetGridPos;
        bool isSafeToKill = gameManager.IsSafePositionToMove(movingPos, myTag);
        movingPiece.isKilling = isSafeToKill;
        return killingPieces[randomNum];
    }

    /// <summary>
    /// Return Objects which can pass or move through given block grid position.
    /// </summary>
    /// <param name="gridPos"></param>
    private Vector2 GetPiecesWhichCanPassThrough(Vector2 gridPos) {
        List<GameObject> availablePieces = gameManager.GetAvailablePieces(myTag);
        List<Vector2> availablePiecesGridPos = gameManager.ConvertGlobPosToGridPos(availablePieces);
        List<Vector2> picesWhichCanPassThrough = gameManager.GetPassingThroughPieces(availablePiecesGridPos, gridPos);
        List<Config.KillingPrioriety> killingPieces = ConvertGridPosToKillingPrioriety(picesWhichCanPassThrough, gridPos);
        return GetLowestPriorietyPiece(killingPieces);
    }

    private Vector2 GetLowestPriorietyPiece(List<Config.KillingPrioriety> killingPriorietyPieces) {
        int kilingPrioerietyPiece = 1;
        Vector2 killingPiece = Vector2.zero;
        for (int i = 0; i < killingPriorietyPieces.Count; ++i) {
            if (killingPriorietyPieces[i].prioriety < kilingPrioerietyPiece)
                continue;
            killingPiece = killingPriorietyPieces[i].pieceGridPos;
        }
        return killingPiece;
    }

    private List<Config.KillingPrioriety> ConvertGridPosToKillingPrioriety(List<Vector2> gridPos, Vector2 targettedPos) {
        List<Config.KillingPrioriety> piecesWithKillingProioriety = new List<Config.KillingPrioriety>();
        for (int i = 0; i < gridPos.Count; ++i) {
            Config.KillingPrioriety killingPrioriety = new Config.KillingPrioriety();
            killingPrioriety.pieceGridPos = gridPos[i];
            killingPrioriety.targetGridPos = targettedPos;
            killingPrioriety.prioriety = gameManager.GetPiecePrioriety(gridPos[i]);
            piecesWithKillingProioriety.Add(killingPrioriety);
        }
        return piecesWithKillingProioriety;
    }
}
