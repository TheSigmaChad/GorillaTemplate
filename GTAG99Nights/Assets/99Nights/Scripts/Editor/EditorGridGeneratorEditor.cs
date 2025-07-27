using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorGridGenerator))]
public class EditorGridGeneratorEditor : Editor
{
    private SerializedProperty tilePrefab;
    private SerializedProperty gridSize;
    private SerializedProperty spacing;
    private SerializedProperty heightVariance;

    private SerializedProperty centerOffset;
    private SerializedProperty numberOfLevels;
    private SerializedProperty levelRadiusStep;
    private SerializedProperty levelMaterials;

    private SerializedProperty markTilesStatic;
    private SerializedProperty clearExistingTiles;

    private bool showGridSettings = true;
    private bool showLevelSettings = true;
    private bool showBatchSettings = true;

    void OnEnable()
    {
        tilePrefab = serializedObject.FindProperty("tilePrefab");
        gridSize = serializedObject.FindProperty("gridSize");
        spacing = serializedObject.FindProperty("spacing");
        heightVariance = serializedObject.FindProperty("heightVariance");

        centerOffset = serializedObject.FindProperty("centerOffset");
        numberOfLevels = serializedObject.FindProperty("numberOfLevels");
        levelRadiusStep = serializedObject.FindProperty("levelRadiusStep");
        levelMaterials = serializedObject.FindProperty("levelMaterials");

        markTilesStatic = serializedObject.FindProperty("markTilesStatic");
        clearExistingTiles = serializedObject.FindProperty("clearExistingTiles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle foldoutStyle = EditorStyles.foldoutHeader;
        foldoutStyle.fontStyle = FontStyle.Bold;

        showGridSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGridSettings, "Grid Settings", foldoutStyle);
        if (showGridSettings)
        {
            EditorGUILayout.PropertyField(tilePrefab);
            EditorGUILayout.PropertyField(gridSize);
            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.PropertyField(heightVariance);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showLevelSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLevelSettings, "Level-Based Materials", foldoutStyle);
        if (showLevelSettings)
        {
            EditorGUILayout.PropertyField(centerOffset);
            EditorGUILayout.PropertyField(numberOfLevels);
            EditorGUILayout.PropertyField(levelRadiusStep);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Materials by Level", EditorStyles.boldLabel);

            levelMaterials.arraySize = Mathf.Max(0, numberOfLevels.intValue);

            for (int i = 0; i < levelMaterials.arraySize; i++)
            {
                SerializedProperty matProp = levelMaterials.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(matProp, new GUIContent($"Level {i} Material"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showBatchSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showBatchSettings, "Batching & Utilities", foldoutStyle);
        if (showBatchSettings)
        {
            EditorGUILayout.PropertyField(markTilesStatic);
            EditorGUILayout.PropertyField(clearExistingTiles);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Click below to generate the tile grid in the editor.", MessageType.Info);
        if (GUILayout.Button("Generate Grid"))
        {
            ((EditorGridGenerator)target).GenerateGrid();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
