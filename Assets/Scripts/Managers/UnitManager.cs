using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.Managers
{
    /// <summary>
    /// Manages all units in the game
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        [Header("Unit Prefabs")]
        public GameObject infantryPrefab;
        public GameObject cavalryPrefab;
        public GameObject artilleryPrefab;

        [Header("Unit Data")]
        public UnitData infantryData;
        public UnitData cavalryData;
        public UnitData artilleryData;

        private List<Unit> allUnits = new List<Unit>();

        public static UnitManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public Unit SpawnUnit(UnitType type, int owner, HexCoordinates position)
        {
            GameObject prefab = GetPrefabForType(type);
            UnitData data = GetDataForType(type);

            if (prefab == null || data == null)
            {
                Debug.LogError($"Missing prefab or data for unit type {type}");
                return null;
            }

            Vector3 worldPos = position.ToWorldPosition(MapManager.Instance.hexSize);
            worldPos.y = 0.5f; // Elevate unit above ground

            GameObject unitObj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            Unit unit = unitObj.GetComponent<Unit>();

            if (unit == null)
                unit = unitObj.AddComponent<Unit>();

            unit.Initialize(data, owner, position);

            // Update cell occupancy
            Cell cell = MapManager.Instance.GetCell(position);
            if (cell != null)
                cell.Occupant = unit;

            allUnits.Add(unit);
            return unit;
        }

        private GameObject GetPrefabForType(UnitType type)
        {
            return type switch
            {
                UnitType.Infantry => infantryPrefab,
                UnitType.Cavalry => cavalryPrefab,
                UnitType.Artillery => artilleryPrefab,
                _ => null
            };
        }

        private UnitData GetDataForType(UnitType type)
        {
            return type switch
            {
                UnitType.Infantry => infantryData,
                UnitType.Cavalry => cavalryData,
                UnitType.Artillery => artilleryData,
                _ => null
            };
        }

        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;

            Cell cell = MapManager.Instance.GetCell(unit.Position);
            if (cell != null && cell.Occupant == unit)
                cell.Occupant = null;

            allUnits.Remove(unit);
            Destroy(unit.gameObject);
        }

        public List<Unit> GetUnitsForPlayer(int playerId)
        {
            return allUnits.FindAll(u => u.Owner == playerId && u.IsAlive());
        }

        public List<Unit> GetAllUnits()
        {
            return new List<Unit>(allUnits);
        }

        public void ResetAllUnitsForNewTurn(int playerId)
        {
            foreach (var unit in GetUnitsForPlayer(playerId))
            {
                unit.ResetForNewTurn();
            }
        }

        public void CleanupDeadUnits()
        {
            List<Unit> deadUnits = allUnits.FindAll(u => !u.IsAlive());
            foreach (var unit in deadUnits)
            {
                RemoveUnit(unit);
            }
        }
    }
}
