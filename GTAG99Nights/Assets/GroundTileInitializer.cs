using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GroundTileInitializer : MonoBehaviour
{
    [System.Serializable]
    public class TileEntry
    {
        public GroundTile tile;
        public int level;
    }

    [Header("Tiles to Initialize")]
    public List<TileEntry> tiles = new List<TileEntry>();

    [ContextMenu("Init All Tiles")]
    public void InitAll()
    {
        int count = 0;
        foreach (var entry in tiles)
        {
            if (entry?.tile == null) continue;
            entry.tile.Init(entry.level);
            count++;
        }
#if UNITY_EDITOR
        Debug.Log($"Initialized {count} GroundTile(s).");
#endif
    }

    [ContextMenu("Remove Nulls")]
    public void RemoveNulls()
    {
        tiles.RemoveAll(t => t == null || t.tile == null);
    }
}
