﻿using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TilesUpdater)), CanEditMultipleObjects]
public class TilesUpdaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TilesUpdater tilesUpdater = (TilesUpdater)target;

        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Tiles"))
        {
            tilesUpdater.ShowTiles(true);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Hide Tiles"))
        {
            tilesUpdater.ShowTiles(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}
