using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorGridGenerator))]
public class EditorGridGeneratorEditor : Editor
{
    private SerializedProperty gridSize;
    private SerializedProperty spacing;

    private SerializedProperty centerOffset;

    private SerializedProperty noiseScale;
    private SerializedProperty minHeight;
    private SerializedProperty maxHeight;
    private SerializedProperty noiseOffset;

    private SerializedProperty numberOfLevels;
    private SerializedProperty levelRadiusStep;
    private SerializedProperty levelPrefabs;

    private SerializedProperty markTilesStatic;
    private SerializedProperty clearExistingTiles;

    private bool showGridSettings = true;
    private bool showHeightSettings = true;
    private bool showLevelSettings = true;
    private bool showBatchSettings = true;

    void OnEnable()
    {
        gridSize = serializedObject.FindProperty("gridSize");
        spacing = serializedObject.FindProperty("spacing");

        centerOffset = serializedObject.FindProperty("centerOffset");

        noiseScale = serializedObject.FindProperty("noiseScale");
        minHeight = serializedObject.FindProperty("minHeight");
        maxHeight = serializedObject.FindProperty("maxHeight");
        noiseOffset = serializedObject.FindProperty("noiseOffset");

        numberOfLevels = serializedObject.FindProperty("numberOfLevels");
        levelRadiusStep = serializedObject.FindProperty("levelRadiusStep");
        levelPrefabs = serializedObject.FindProperty("levelPrefabs");

        markTilesStatic = serializedObject.FindProperty("markTilesStatic");
        clearExistingTiles = serializedObject.FindProperty("clearExistingTiles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle foldoutStyle = EditorStyles.foldoutHeader;
        foldoutStyle.fontStyle = FontStyle.Bold;

        // Grid Settings
        showGridSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGridSettings, "Grid Settings", foldoutStyle);
        if (showGridSettings)
        {
            EditorGUILayout.PropertyField(gridSize);
            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.PropertyField(centerOffset);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Height Settings (Perlin Noise)
        showHeightSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showHeightSettings, "Height Noise Settings", foldoutStyle);
        if (showHeightSettings)
        {
            EditorGUILayout.PropertyField(noiseScale);
            EditorGUILayout.PropertyField(minHeight);
            EditorGUILayout.PropertyField(maxHeight);
            EditorGUILayout.PropertyField(noiseOffset);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Level-based prefab spawning
        showLevelSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLevelSettings, "Level-Based Prefabs", foldoutStyle);
        if (showLevelSettings)
        {
            EditorGUILayout.PropertyField(numberOfLevels);
            EditorGUILayout.PropertyField(levelRadiusStep);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Prefabs by Level", EditorStyles.boldLabel);

            levelPrefabs.arraySize = Mathf.Max(0, numberOfLevels.intValue);

            for (int i = 0; i < levelPrefabs.arraySize; i++)
            {
                SerializedProperty prefabProp = levelPrefabs.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(prefabProp, new GUIContent($"Level {i} Prefab"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Batching / utility settings
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
