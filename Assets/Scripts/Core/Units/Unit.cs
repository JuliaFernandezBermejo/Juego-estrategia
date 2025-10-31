using UnityEngine;
using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.Core.Units
{
    /// <summary>
    /// Represents a unit on the battlefield
    /// </summary>
    public class Unit : MonoBehaviour
    {
        public UnitData Data { get; private set; }
        public int Owner { get; private set; }
        public HexCoordinates Position { get; set; }

        public int CurrentHealth { get; private set; }
        public int RemainingMovement { get; private set; }
        public bool HasAttacked { get; private set; }

        public void Initialize(UnitData data, int owner, HexCoordinates position)
        {
            Data = data;
            Owner = owner;
            Position = position;
            CurrentHealth = data.health;
            RemainingMovement = data.movementPoints;
            HasAttacked = false;
        }

        public void ResetForNewTurn()
        {
            RemainingMovement = Data.movementPoints;
            HasAttacked = false;
        }

        public void Move(HexCoordinates newPosition, int movementCost)
        {
            Position = newPosition;
            RemainingMovement -= movementCost;
        }

        public bool CanMove()
        {
            return RemainingMovement > 0;
        }

        public bool CanAttack(HexCoordinates target)
        {
            if (HasAttacked) return false;
            int distance = HexCoordinates.Distance(Position, target);
            return distance <= Data.attackRange;
        }

        public void Attack(Unit target, TerrainType attackerTerrain, TerrainType defenderTerrain)
        {
            // Calculate attack with terrain modifiers
            float attackPower = Data.attackPoints * Data.GetTerrainModifier(attackerTerrain);
            float defensePower = target.Data.defensePoints * target.Data.GetTerrainModifier(defenderTerrain);
            defensePower *= (1f + defenderTerrain.GetDefenseBonus());

            int damage = Mathf.Max(1, Mathf.RoundToInt(attackPower - defensePower * 0.5f));
            target.TakeDamage(damage);
            HasAttacked = true;
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }

    // Extension method for TerrainType
    public static class TerrainTypeExtensions
    {
        public static float GetDefenseBonus(this TerrainType terrain)
        {
            return terrain switch
            {
                TerrainType.Plains => 0f,
                TerrainType.Forest => 0.2f,
                TerrainType.Mountain => 0.4f,
                _ => 0f
            };
        }
    }
}
