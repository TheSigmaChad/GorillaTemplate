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

    [Header("Height Noise Settings")]
    [Tooltip("Controls the smoothness of the height changes.")]
    public float noiseScale = 0.1f;
    [Tooltip("Minimum height of the terrain.")]
    public float minHeight = -2f;
    [Tooltip("Maximum height of the terrain.")]
    public float maxHeight = 2f;
    [Tooltip("Noise offset to shift the terrain pattern.")]
    public float noiseOffset = 0f;

    [Header("Level-Based Variation")]
    [Tooltip("Total number of levels for prefab changes.")]
    public int numberOfLevels = 5;
    public float levelRadiusStep = 10f;
    public GameObject[] levelPrefabs;

    [Header("Batching Options")]
    public bool markTilesStatic = true;
    public bool clearExistingTiles = true;

    public void GenerateGrid()
    {
        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("Level Prefabs array is empty!");
            return;
        }

        if (clearExistingTiles)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        float offset = (gridSize - 1) * spacing / 2f;
        Vector3 center = centerOffset;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Generate Perlin noise-based height
                float noiseX = x * noiseScale + noiseOffset;
                float noiseZ = z * noiseScale + noiseOffset;
                float noiseValue = Mathf.PerlinNoise(noiseX, noiseZ); // [0, 1]
                float yOffset = Mathf.Lerp(minHeight, maxHeight, noiseValue);

                Vector3 pos = new Vector3(x * spacing - offset, yOffset, z * spacing - offset) + centerOffset;

                // Determine level by distance from center
                float dist = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(center.x, 0, center.z));
                int level = Mathf.Clamp(Mathf.FloorToInt(dist / levelRadiusStep), 0, numberOfLevels - 1);

                // Choose appropriate prefab
                GameObject prefab = (level < levelPrefabs.Length && levelPrefabs[level] != null)
                    ? levelPrefabs[level]
                    : levelPrefabs[0]; // fallback to first prefab if null or missing

                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                tile.transform.SetParent(transform);
                tile.transform.position = pos;
                tile.transform.rotation = Quaternion.identity;
                tile.transform.localScale = Vector3.one;

                // Init the tile with level info
                GroundTile tileScript = tile.GetComponent<GroundTile>();
                if (tileScript != null)
                {
                    tileScript.Init(level);
                }

                tile.isStatic = markTilesStatic;
                Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
            }
        }

        Debug.Log($"Generated {gridSize * gridSize} tiles using {levelPrefabs.Length} level-based prefabs with Perlin noise height variation.");
    }
}
#endif
