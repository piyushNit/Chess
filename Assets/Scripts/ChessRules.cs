using UnityEngine;
using System.Collections;

public class ChessRules{
	public static string[] chessPiecesPlacement = new string[] {
		Config.ROOK, Config.KNIGHT, Config.BISHOP,
		Config.QUEEN, Config.KING,
		Config.BISHOP, Config.KNIGHT, Config.ROOK,
		Config.PAWN
	};
}
