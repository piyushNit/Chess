using UnityEngine;
using System.Collections;

public class MainMenuCreator : MonoBehaviour {
    [SerializeField] private GameObject[] objects;
    [SerializeField] private int minX;
    [SerializeField] private int maxX;
    [SerializeField] private int minZ;
    [SerializeField] private int maxZ;
    [SerializeField] private int totalPieces;

    public void DrawPieces() {
        if (objects.Length == 0)
            return;
        for (int i = 0; i < totalPieces; ++i) {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            Vector3 newPosition = new Vector3(x, 0, z);
            int index = Random.Range(0, objects.Length);
            GameObject obj = Instantiate(objects[index], newPosition, Quaternion.identity) as GameObject;
            obj.GetComponentInChildren<Renderer>().material.color = Color.white;
            obj.transform.parent = this.transform;
        }
    }
}
