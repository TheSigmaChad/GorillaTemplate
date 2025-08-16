using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("Tile Properties")]
    public TileType tileType;

    [Header("Spawn Area")]
    [SerializeField] private Transform groundPlane;
    [SerializeField] private List<TileSpawnable> spawnables;

    [Header("Randomization")]
    [SerializeField] private int minSpawnCount = 10;
    [SerializeField] private int maxSpawnCount = 25;

    public void Init(int level)
    {
        if (groundPlane == null)
        {
            Debug.LogWarning("Ground plane is not assigned.");
            return;
        }

        List<TileSpawnable> validSpawnables = GetValidSpawnables(level);
        if (validSpawnables.Count == 0) return;

        List<Bounds> placedBounds = new List<Bounds>();
        Vector3 origin = groundPlane.position;
        Vector3 size = groundPlane.localScale * 10f; // adjust as needed for your tile system

        int totalSpawns = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < totalSpawns; i++)
        {
            TileSpawnable chosen = validSpawnables[Random.Range(0, validSpawnables.Count)];
            if (chosen?.prefab == null) continue;

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
