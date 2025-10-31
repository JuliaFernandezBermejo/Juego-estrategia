namespace TurnBasedStrategy.Core.Resources
{
    public enum StructureType
    {
        Barracks,  // Produces Infantry and Cavalry
        Factory    // Produces Artillery
    }

    /// <summary>
    /// Represents a production building
    /// </summary>
    public class Structure
    {
        public StructureType Type { get; private set; }
        public int Owner { get; private set; }

        public Structure(StructureType type, int owner)
        {
            Type = type;
            Owner = owner;
        }
    }
}
