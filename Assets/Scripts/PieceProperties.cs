using UnityEngine;

public class PieceProperties : MonoBehaviour {
    [HideInInspector] public string typeStr;
    public bool isPawn;
    [HideInInspector] public bool canMoveTwoBlocks = true;
    [HideInInspector] public bool isDead = false;
    private int moves = 0;
    public int getMoves {get {return moves;}}

    public void UpdateMoves() {
        moves++;
    }

    public void UndoMoves() {
        moves--;
        if (moves > 0)
            return;
        moves = 0;
        canMoveTwoBlocks = true;
        UpdateIsDead();
    }

    private void UpdateIsDead() {
        if (isDead)
            isDead = false;
    }
}
