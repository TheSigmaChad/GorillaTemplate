using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TileSpawnableManager : EditorWindow
{
    private List<TileSpawnable> spawnables;
    private TileType selectedTileType = TileType.Grass;
    private Vector2 scrollPos;
    private Dictionary<TileSpawnable, bool> foldouts = new();

    [MenuItem("Tools/Tile Spawnable Manager")]
    public static void ShowWindow()
    {
        GetWindow<TileSpawnableManager>("Tile Spawnables");
    }

    private void OnEnable()
    {
        LoadSpawnables();
    }

    private void LoadSpawnables()
    {
        spawnables = AssetDatabase.FindAssets("t:TileSpawnable")
            .Select(guid => AssetDatabase.LoadAssetAtPath<TileSpawnable>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();

        // Initialize foldout state
        foreach (var s in spawnables)
        {
            if (!foldouts.ContainsKey(s))
                foldouts[s] = false;
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Tile Spawnable Manager", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Spawnable", GUILayout.Height(25)))
        {
            CreateNewSpawnable();
        }

        GUILayout.FlexibleSpace();
        selectedTileType = (TileType)EditorGUILayout.EnumPopup("Filter by Type", selectedTileType);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var spawnable in spawnables.Where(s => s.TileType == selectedTileType))
        {
            if (spawnable == null) continue;

            foldouts[spawnable] = EditorGUILayout.Foldout(foldouts[spawnable], spawnable.name, true, EditorStyles.foldoutHeader);

            if (foldouts[spawnable])
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Space(4);

                EditorGUI.BeginChangeCheck();

                spawnable.TileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", spawnable.TileType);
                spawnable.MinLevel = EditorGUILayout.IntField("Min Level", spawnable.MinLevel);
                spawnable.MaxLevel = EditorGUILayout.IntField("Max Level", spawnable.MaxLevel);
                spawnable.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", spawnable.prefab, typeof(GameObject), false);

                if (spawnable.prefab != null)
                {
                    Texture2D preview = AssetPreview.GetAssetPreview(spawnable.prefab);
                    if (preview != null)
                    {
                        GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spawnable);
                }

                GUILayout.Space(4);
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateNewSpawnable()
    {
        string path = EditorUtility.SaveFilePanelInProject("Create Tile Spawnable", "NewTileSpawnable", "asset", "Enter a name");
        if (!string.IsNullOrEmpty(path))
        {
            TileSpawnable newSpawnable = ScriptableObject.CreateInstance<TileSpawnable>();
            newSpawnable.TileType = selectedTileType;
            AssetDatabase.CreateAsset(newSpawnable, path);
            AssetDatabase.SaveAssets();
            LoadSpawnables();
        }
    }
}
