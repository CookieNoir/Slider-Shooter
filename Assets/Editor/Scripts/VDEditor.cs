using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(VDTest))]
public class VDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VDTest vd = (VDTest)target;
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
