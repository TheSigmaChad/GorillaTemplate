using UnityEngine;

[ExecuteInEditMode]
public class TerrainLevelPainter : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;
    public int terrainSize = 512; // X and Z size (must be equal for square tiles)
    public int heightmapResolution = 513; // Must be 2^n + 1 (e.g. 513, 1025)
    public int alphamapResolution = 512;

    [Header("Height Generation")]
    public float noiseScale = 0.03f;
    public float heightMultiplier = 10f;

    [Header("Material Levels")]
    public TerrainLayer[] levelLayers; // Assign in inspector
    public int numberOfLevels = 5;
    public float levelRadiusStep = 50f;
    public Vector3 centerOffset = Vector3.zero;

    public void Generate()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned.");
            return;
        }

        if (levelLayers == null || levelLayers.Length == 0)
        {
            Debug.LogError("No terrain layers assigned.");
            return;
        }

        TerrainData data = terrain.terrainData;
        data.heightmapResolution = heightmapResolution;
        data.alphamapResolution = alphamapResolution;
        data.size = new Vector3(terrainSize, heightMultiplier * 10, terrainSize); // Ensure square XZ

        GenerateHeights(data);
        PaintLevels(data);
        terrain.Flush();
    }

    void GenerateHeights(TerrainData data)
    {
        int res = data.heightmapResolution;
        float[,] heights = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float worldX = (float)x / res * terrainSize;
                float worldZ = (float)y / res * terrainSize;
                float noise = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale);
                heights[y, x] = noise * heightMultiplier / data.size.y;
            }
        }

        data.SetHeights(0, 0, heights);
    }

    void PaintLevels(TerrainData data)
    {
        int res = data.alphamapResolution;
        float[,,] splatmap = new float[res, res, levelLayers.Length];
        Vector3 center = new Vector3(terrainSize / 2f, 0, terrainSize / 2f) + centerOffset;

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float normX = (float)x / (res - 1);
                float normZ = (float)z / (res - 1);

                float worldX = normX * terrainSize;
                float worldZ = normZ * terrainSize;

                float distance = Vector2.Distance(new Vector2(worldX, worldZ), new Vector2(center.x, center.z));
                int level = Mathf.Clamp(Mathf.FloorToInt(distance / levelRadiusStep), 0, numberOfLevels - 1);

                for (int i = 0; i < levelLayers.Length; i++)
                {
                    splatmap[z, x, i] = (i == level) ? 1f : 0f;
                }
            }
        }

        data.terrainLayers = levelLayers;
        data.SetAlphamaps(0, 0, splatmap);
    }

    public void Clear()
    {
        if (terrain == null) return;

        int res = terrain.terrainData.heightmapResolution;
        float[,] flatHeights = new float[res, res];
        terrain.terrainData.SetHeights(0, 0, flatHeights);

        int alphaRes = terrain.terrainData.alphamapResolution;
        float[,,] clearAlpha = new float[alphaRes, alphaRes, levelLayers.Length];

        // Optional: default all to first layer
        for (int x = 0; x < alphaRes; x++)
        {
            for (int y = 0; y < alphaRes; y++)
            {
                clearAlpha[x, y, 0] = 1f;
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, clearAlpha);

        Debug.Log("Terrain cleared.");
    }

}
