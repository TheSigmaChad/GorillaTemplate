using UnityEngine;

[CreateAssetMenu(fileName = "TileSpawnable", menuName = "Tile/New Object", order = 100)]
public class TileSpawnable : ScriptableObject
{
    public TileType TileType;          // New: Used for filtering
    public int MinLevel;
    public int MaxLevel;
    public GameObject prefab;
}
