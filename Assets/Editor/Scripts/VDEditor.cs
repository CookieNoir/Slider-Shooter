using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(VoronoiDiagramHandler))]
public class VDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VoronoiDiagramHandler vd = (VoronoiDiagramHandler)target;
        if (GUILayout.Button("Create diagram"))
        {
            vd.CreateVoronoiDiagram();
        }
        if (GUILayout.Button("Remove diagram"))
        {
            vd.DropDiagram();
        }
    }
}
