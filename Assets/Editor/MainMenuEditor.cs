using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainMenuCreator))]
public class MainMenuEditor : Editor {
    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //    MainMenuCreator map = target as MainMenuCreator;
    //    if (DrawDefaultInspector())
    //    {
    //        map.DrawPieces();
    //    }

    //    if (GUILayout.Button("Generate new Map"))
    //    {
    //        map.DrawPieces();
    //    }
    //}
}
