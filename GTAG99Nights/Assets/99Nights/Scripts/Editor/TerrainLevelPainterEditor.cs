using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainLevelPainter))]
public class TerrainLevelPainterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainLevelPainter script = (TerrainLevelPainter)target;

        GUILayout.Space(10);
        if (GUILayout.Button("🌱 Generate Terrain"))
        {
            script.Generate();
        }

        if (GUILayout.Button("🧹 Clear Terrain"))
        {
            if (EditorUtility.DisplayDialog("Clear Terrain?", "This will reset the terrain heights and clear texture layers. Are you sure?", "Yes", "Cancel"))
            {
                script.Clear();
            }
        }
    }
}
