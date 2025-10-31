using UnityEngine;

namespace TurnBasedStrategy.Core.Map
{
    /// <summary>
    /// Represents hexagonal coordinates using axial coordinate system (q, r)
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {
        public int q; // Column
        public int r; // Row

        public HexCoordinates(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        // Cube coordinates for easier distance calculations
        public int S => -q - r;

        // Distance between two hex coordinates
        public static int Distance(HexCoordinates a, HexCoordinates b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.S - b.S)) / 2;
        }

        // Get 6 neighboring hex coordinates
        public static HexCoordinates[] GetNeighbors(HexCoordinates hex)
        {
            return new HexCoordinates[]
            {
                new HexCoordinates(hex.q + 1, hex.r),     // Right
                new HexCoordinates(hex.q + 1, hex.r - 1), // Top-right
                new HexCoordinates(hex.q, hex.r - 1),     // Top-left
                new HexCoordinates(hex.q - 1, hex.r),     // Left
                new HexCoordinates(hex.q - 1, hex.r + 1), // Bottom-left
                new HexCoordinates(hex.q, hex.r + 1)      // Bottom-right
            };
        }

        // Convert axial coordinates to world position
        public Vector3 ToWorldPosition(float size = 1f)
        {
            float x = size * (1.5f * q);
            float z = size * (Mathf.Sqrt(3f) * r + Mathf.Sqrt(3f) / 2f * q);
            return new Vector3(x, 0, z);
        }

        public override bool Equals(object obj)
        {
            if (obj is HexCoordinates other)
                return q == other.q && r == other.r;
            return false;
        }

        public override int GetHashCode()
        {
            return (q, r).GetHashCode();
        }

        public static bool operator ==(HexCoordinates a, HexCoordinates b)
        {
            return a.q == b.q && a.r == b.r;
        }

        public static bool operator !=(HexCoordinates a, HexCoordinates b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"Hex({q}, {r})";
        }
    }
}
