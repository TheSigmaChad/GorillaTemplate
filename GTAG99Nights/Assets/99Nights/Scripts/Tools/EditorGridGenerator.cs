using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class EditorGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 72;
    public float spacing = 1f;

    [Header("Positioning")]
    public Vector3 centerOffset = Vector3.zero;
    [Tooltip("All tiles will be placed at this Y height (flat).")]
    public float flatY = 0f;

    [Header("Level-Based Variation")]
    [Tooltip("Total number of levels for prefab changes.")]
    public int numberOfLevels = 5;
    public float levelRadiusStep = 10f;
    public GameObject[] levelPrefabs;

    [Header("Batching Options")]
    public bool markTilesStatic = true;
    public bool clearExistingTiles = true;

    [Header("Initialization Output")]
    [Tooltip("Will be auto-created/linked and populated with the tiles + levels.")]
    public GroundTileInitializer initializer;

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("Level Prefabs array is empty!");
            return;
        }

        // Ensure initializer exists
        if (initializer == null)
        {
            initializer = GetComponent<GroundTileInitializer>();
            if (initializer == null)
            {
                initializer = gameObject.AddComponent<GroundTileInitializer>();
                Undo.RegisterCreatedObjectUndo(initializer, "Add GroundTileInitializer");
            }
        }

        if (clearExistingTiles)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // Clear initializer list for fresh population
        initializer.tiles.Clear();

        float offset = (gridSize - 1) * spacing / 2f;
        Vector3 center = centerOffset;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Flat placement (no noise)
                Vector3 pos = new Vector3(x * spacing - offset, flatY, z * spacing - offset) + centerOffset;

                // Level by distance from center
                float dist = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(center.x, 0, center.z));
                int level = Mathf.Clamp(Mathf.FloorToInt(dist / levelRadiusStep), 0, numberOfLevels - 1);

                // Choose appropriate prefab
                GameObject prefab = (level < levelPrefabs.Length && levelPrefabs[level] != null)
                    ? levelPrefabs[level]
                    : levelPrefabs[0];

                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                tile.transform.SetParent(transform);
                tile.transform.position = pos;
                tile.transform.rotation = Quaternion.identity;
                tile.transform.localScale = Vector3.one;

                // Record in initializer (no Init call here)
                var gt = tile.GetComponent<GroundTile>();
                if (gt != null)
                {
                    initializer.tiles.Add(new GroundTileInitializer.TileEntry
                    {
                        tile = gt,
                        level = level
                    });
                }

                tile.isStatic = markTilesStatic;
                Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
            }
        }

        Debug.Log($"Generated {gridSize * gridSize} tiles and populated GroundTileInitializer with {initializer.tiles.Count} entries (no Init calls made).");
    }
}
#endif
