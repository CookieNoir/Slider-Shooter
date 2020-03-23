using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(VoronoiWallHandler))]
public class VoronoiWallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VoronoiWallHandler voronoiWall = (VoronoiWallHandler)target;
        if (GUILayout.Button("Create diagram"))
        {
            voronoiWall.CreateVoronoiDiagram();
        }
        if (GUILayout.Button("Remove diagram"))
        {
            voronoiWall.DropDiagram();
        }
    }
}
