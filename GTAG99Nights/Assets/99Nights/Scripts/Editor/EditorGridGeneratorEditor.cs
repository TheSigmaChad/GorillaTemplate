using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorGridGenerator))]
public class EditorGridGeneratorEditor : Editor
{
    // Grid
    private SerializedProperty gridSize;
    private SerializedProperty spacing;

    // Positioning / Height
    private SerializedProperty centerOffset;
    private SerializedProperty flatY;

    // Levels
    private SerializedProperty numberOfLevels;
    private SerializedProperty levelRadiusStep;
    private SerializedProperty levelPrefabs;

    // Batching
    private SerializedProperty markTilesStatic;
    private SerializedProperty clearExistingTiles;

    // Init output
    private SerializedProperty initializer;

    // Foldouts
    private bool showGridSettings = true;
    private bool showPositioningSettings = true;
    private bool showLevelSettings = true;
    private bool showBatchSettings = true;
    private bool showInitSettings = true;

    void OnEnable()
    {
        // Grid
        gridSize = serializedObject.FindProperty("gridSize");
        spacing  = serializedObject.FindProperty("spacing");

        // Positioning / Height
        centerOffset = serializedObject.FindProperty("centerOffset");
        flatY        = serializedObject.FindProperty("flatY");

        // Levels
        numberOfLevels   = serializedObject.FindProperty("numberOfLevels");
        levelRadiusStep  = serializedObject.FindProperty("levelRadiusStep");
        levelPrefabs     = serializedObject.FindProperty("levelPrefabs");

        // Batching
        markTilesStatic   = serializedObject.FindProperty("markTilesStatic");
        clearExistingTiles = serializedObject.FindProperty("clearExistingTiles");

        // Init output
        initializer = serializedObject.FindProperty("initializer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var boldFoldout = new GUIStyle(EditorStyles.foldoutHeader) { fontStyle = FontStyle.Bold };

        // Grid
        showGridSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGridSettings, "Grid Settings", boldFoldout);
        if (showGridSettings)
        {
            EditorGUILayout.PropertyField(gridSize);
            EditorGUILayout.PropertyField(spacing);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Positioning / Flat Height
        showPositioningSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showPositioningSettings, "Positioning & Height", boldFoldout);
        if (showPositioningSettings)
        {
            EditorGUILayout.PropertyField(centerOffset);
            EditorGUILayout.PropertyField(flatY, new GUIContent("Flat Y (Height)"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Level-based prefabs
        showLevelSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLevelSettings, "Level-Based Prefabs", boldFoldout);
        if (showLevelSettings)
        {
            EditorGUILayout.PropertyField(numberOfLevels);
            EditorGUILayout.PropertyField(levelRadiusStep);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Prefabs by Level", EditorStyles.boldLabel);

            // Ensure array size matches level count
            if (numberOfLevels.intValue < 0) numberOfLevels.intValue = 0;
            if (levelPrefabs.arraySize != numberOfLevels.intValue)
            {
                levelPrefabs.arraySize = numberOfLevels.intValue;
            }

            for (int i = 0; i < levelPrefabs.arraySize; i++)
            {
                var prefabProp = levelPrefabs.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(prefabProp, new GUIContent($"Level {i} Prefab"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Batching / Utilities
        showBatchSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showBatchSettings, "Batching & Utilities", boldFoldout);
        if (showBatchSettings)
        {
            EditorGUILayout.PropertyField(markTilesStatic);
            EditorGUILayout.PropertyField(clearExistingTiles);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Initialization output / actions
        showInitSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showInitSettings, "Initialization Output", boldFoldout);
        if (showInitSettings)
        {
            EditorGUILayout.PropertyField(initializer, new GUIContent("Ground Tile Initializer"));

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate Grid", GUILayout.Height(26)))
                {
                    ((EditorGridGenerator)target).GenerateGrid();
                }

                using (new EditorGUI.DisabledScope(initializer.objectReferenceValue == null))
                {
                    if (GUILayout.Button("Init All Tiles", GUILayout.Height(26)))
                    {
                        var init = initializer.objectReferenceValue as GroundTileInitializer;
                        if (init != null)
                        {
                            init.InitAll();
                        }
                    }
                }
            }

            if (initializer.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("The initializer will be auto-created and linked when you Generate Grid.", MessageType.Info);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
