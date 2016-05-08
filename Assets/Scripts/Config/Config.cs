using UnityEngine;

public class Config {
	public static int BOARD_BLOCKS = 8;
	public static int TOTAL_BLOCKS = 64;

	public static string PAWN = "PAWN";
	public static string KING = "KING";
	public static string QUEEN = "QUEEN";
	public static string ROOK = "ROOK";
	public static string BISHOP = "BISHOP";
	public static string KNIGHT = "KNIGHT";

    public static int PAWN_FIRST_TIME_MOVE = 2;
    public static int PAWN_MOVE = 1;
    public static int KING_MOVE = 1;
    public static int QUEEN_MOVE = 8;
    public static int ROOK_MOVE = 8;
    public static int BISHOP_MOVE = 8;
    public static int KNIGHT_MOVE = 2;

    public static string WHITE_TAG = "White";
    public static string BLACK_TAG = "Black";

    public static int SUPREME_PRIORITY = 4;
    public static int HIGH_PRIORITY = 3;
    public static int MEDIUM_PRIORITY = 2;
    public static int LOW_PRIORITY = 1;
    public static int NO_PRIORITY = 0;

    public static float SELECT_UP_MOVE = 1.2f;

    public struct KillingPrioriety
    {
        public int prioriety;
        public Vector2 pieceGridPos;
        public Vector2 targetGridPos;
    }
}
