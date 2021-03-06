﻿using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GameSettings)), CanEditMultipleObjects]
public class GameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameSettings gameSettings = (GameSettings)target;
        if (GUILayout.Button("Fill Tiles (Requires Visible Tiles)"))
        {
            gameSettings.FillTiles();
        }
        DrawDefaultInspector();
    }
}
