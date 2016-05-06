using UnityEngine;

public class PieceProperties : MonoBehaviour {
    [HideInInspector] public string typeStr;
    public bool isPawn;
    [HideInInspector] public bool canMoveTwoBlocks = true;
    [HideInInspector] public bool isDead = false;
}
