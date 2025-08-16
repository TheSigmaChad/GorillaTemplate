using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("Tile Properties")]
    public TileType tileType;

    [SerializeField] public bool CanSpawnEntities = true;
    [SerializeField] public bool FlatGround = false;

    [Header("Spawn Area")]
    [SerializeField] private Transform groundPlane;
    [SerializeField] private List<TileSpawnable> spawnables;

    [Header("Randomization")]
    [SerializeField] private int minSpawnCount = 1;
    [SerializeField] private int maxSpawnCount = 3;
    [SerializeField] private int spawnChance = 75;

    public void Init(int level)
    {
        if (groundPlane == null)
        {
            Debug.LogWarning("Ground plane is not assigned.");
            return;
        }

        if (CanSpawnEntities)
        {
            // CreateSpawnables(level);
        }

        if (!FlatGround)
        {
            // Change the shaders position material property block
            OffsetTile();
        }


    }

    private void CreateSpawnables(int level)
{
    List<TileSpawnable> validSpawnables = GetValidSpawnables(level);
    if (validSpawnables.Count == 0) return;

    List<Bounds> placedBounds = new List<Bounds>();
    Vector3 origin = groundPlane.position;
    Vector3 size = groundPlane.localScale * 10f;

    int totalSpawns = Random.Range(minSpawnCount, maxSpawnCount + 1);

    for (int i = 0; i < totalSpawns; i++)
    {
        // First roll global chance
        if (Random.Range(0, 100) > spawnChance) // <-- you can expose this per-tile too
            continue;

        // Weighted choice among spawnables that pass their own SpawnChance
        TileSpawnable chosen = GetWeightedRandomSpawnable(validSpawnables);
        if (chosen == null || chosen.prefab == null) continue;

        const int maxAttempts = 10;
        bool placed = false;

        for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
        {
            Vector3 randomPos = GetRandomPositionInTile(origin, size);
            Bounds newBounds = GetPrefabBounds(chosen.prefab, randomPos);

            if (!OverlapsExisting(newBounds, placedBounds))
            {
                GameObject obj = Instantiate(chosen.prefab, randomPos, Quaternion.identity, transform);
                placedBounds.Add(GetObjectBounds(obj));
                placed = true;
            }
        }
    }
}

private TileSpawnable GetWeightedRandomSpawnable(List<TileSpawnable> candidates)
{
    // Filter by per-spawnable chance
    List<TileSpawnable> filtered = candidates.FindAll(s => Random.Range(0, 100) < s.SpawnChance);
    if (filtered.Count == 0) return null;

    int totalWeight = 0;
    foreach (var s in filtered) totalWeight += s.Weight;

    int roll = Random.Range(0, totalWeight);
    foreach (var s in filtered)
    {
        if (roll < s.Weight) return s;
        roll -= s.Weight;
    }

    return null;
}


    private void OffsetTile()
    {
        // Offset the tile
    }

    private List<TileSpawnable> GetValidSpawnables(int level)
    {
        return spawnables.FindAll(s =>
            level >= s.MinLevel &&
            level <= s.MaxLevel &&
            s.TileType == tileType &&
            s.prefab != null);
    }

    private Vector3 GetRandomPositionInTile(Vector3 origin, Vector3 size)
    {
        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float z = Random.Range(-size.z / 2f, size.z / 2f);
        return origin + new Vector3(x, 0f, z);
    }

    private bool OverlapsExisting(Bounds newBounds, List<Bounds> placedBounds)
    {
        foreach (var b in placedBounds)
        {
            if (b.Intersects(newBounds)) return true;
        }
        return false;
    }

    private Bounds GetPrefabBounds(GameObject prefab, Vector3 position)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(position, Vector3.zero);

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Vector3 offset = position - bounds.center;
        bounds.center += offset;

        return bounds;
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }
}
