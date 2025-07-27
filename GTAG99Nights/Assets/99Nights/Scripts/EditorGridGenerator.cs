using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int gridSize = 72;
    public float spacing = 1f;
    public float heightVariance = 0.2f;

    [Header("Positioning")]
    public Vector3 centerOffset = Vector3.zero;

    [Header("Level-Based Variation")]
    [Tooltip("Total number of levels for material changes.")]
    public int numberOfLevels = 5;
    public float levelRadiusStep = 10f;
    public Material[] levelMaterials;

    [Header("Batching Options")]
    public bool markTilesStatic = true;
    public bool clearExistingTiles = true;

    public void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab not assigned!");
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
                float yOffset = Random.Range(-heightVariance, heightVariance);
                Vector3 pos = new Vector3(x * spacing - offset, yOffset, z * spacing - offset) + centerOffset;

                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
                tile.transform.SetParent(transform);
                tile.transform.position = pos;
                tile.transform.rotation = Quaternion.identity;
                tile.transform.localScale = Vector3.one;

                // Determine level by distance
                float dist = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(center.x, 0, center.z));
                int level = Mathf.Clamp(Mathf.FloorToInt(dist / levelRadiusStep), 0, numberOfLevels - 1);

                // Apply material from levelMaterials if available
                var renderer = tile.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material selectedMaterial = (levelMaterials != null && level < levelMaterials.Length && levelMaterials[level] != null)
                        ? levelMaterials[level]
                        : tilePrefab.GetComponent<MeshRenderer>().sharedMaterial;

                    renderer.sharedMaterial = selectedMaterial;
                }

                tile.isStatic = markTilesStatic;
                Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
            }
        }

        Debug.Log($"Generated {gridSize * gridSize} tiles with {numberOfLevels} material levels.");
    }
}
