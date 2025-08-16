using UnityEngine;

[CreateAssetMenu(fileName = "TileSpawnable", menuName = "Tile/New Object", order = 100)]
public class TileSpawnable : ScriptableObject
{
    public TileType TileType;
    public int MinLevel;
    public int MaxLevel;
    public GameObject prefab;

    [Range(0, 100)] public int SpawnChance = 100; // percent chance to try spawning
    [Range(1, 100)] public int Weight = 1;        // relative weight in selection
}
