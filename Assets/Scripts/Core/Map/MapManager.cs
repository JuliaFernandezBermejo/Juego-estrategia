using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Core.Resources;

namespace TurnBasedStrategy.Core.Map
{
    /// <summary>
    /// Manages the hexagonal grid map
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        public int mapRadius = 5; // Creates a hexagonal map
        public float hexSize = 1f;

        [Header("Prefabs")]
        public GameObject plainsTilePrefab;
        public GameObject forestTilePrefab;
        public GameObject mountainTilePrefab;

        private Dictionary<HexCoordinates, Cell> cells = new Dictionary<HexCoordinates, Cell>();
        private Dictionary<HexCoordinates, GameObject> tileObjects = new Dictionary<HexCoordinates, GameObject>();

        public static MapManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void GenerateMap()
        {
            cells.Clear();

            // Generate hexagonal map
            for (int q = -mapRadius; q <= mapRadius; q++)
            {
                int r1 = Mathf.Max(-mapRadius, -q - mapRadius);
                int r2 = Mathf.Min(mapRadius, -q + mapRadius);

                for (int r = r1; r <= r2; r++)
                {
                    HexCoordinates hex = new HexCoordinates(q, r);
                    TerrainType terrain = GenerateTerrainType(hex);
                    Cell cell = new Cell(hex, terrain);

                    // Add some resource cells (10% chance)
                    if (Random.value < 0.1f)
                    {
                        cell.ResourceOnCell = (ResourceType)Random.Range(0, 3);
                    }

                    cells[hex] = cell;
                    CreateTileVisual(hex, terrain);
                }
            }
        }

        private TerrainType GenerateTerrainType(HexCoordinates hex)
        {
            // Simple procedural terrain generation
            float noise = Mathf.PerlinNoise(hex.q * 0.1f, hex.r * 0.1f);

            if (noise < 0.3f) return TerrainType.Mountain;
            if (noise < 0.6f) return TerrainType.Forest;
            return TerrainType.Plains;
        }

        private void CreateTileVisual(HexCoordinates hex, TerrainType terrain)
        {
            GameObject prefab = terrain switch
            {
                TerrainType.Plains => plainsTilePrefab,
                TerrainType.Forest => forestTilePrefab,
                TerrainType.Mountain => mountainTilePrefab,
                _ => plainsTilePrefab
            };

            if (prefab != null)
            {
                Vector3 position = hex.ToWorldPosition(hexSize);
                GameObject tile = Instantiate(prefab, position, Quaternion.identity, transform);
                tile.name = $"Hex_{hex.q}_{hex.r}";
                tileObjects[hex] = tile;
            }
        }

        public Cell GetCell(HexCoordinates coords)
        {
            return cells.TryGetValue(coords, out Cell cell) ? cell : null;
        }

        public bool IsValidCoordinate(HexCoordinates coords)
        {
            return cells.ContainsKey(coords);
        }

        public List<HexCoordinates> GetNeighbors(HexCoordinates hex)
        {
            List<HexCoordinates> neighbors = new List<HexCoordinates>();
            foreach (var neighbor in HexCoordinates.GetNeighbors(hex))
            {
                if (IsValidCoordinate(neighbor))
                    neighbors.Add(neighbor);
            }
            return neighbors;
        }

        public List<Cell> GetAllCells()
        {
            return new List<Cell>(cells.Values);
        }

        public int GetTotalCells()
        {
            return cells.Count;
        }
    }
}
