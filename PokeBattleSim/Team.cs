namespace PokeBattleSim
{
    /// <summary>
    /// A team of pokemon.
    /// </summary>
    public class Team
    {
        private static readonly Random _s_rng = new();

        public static readonly IDictionary<uint, Team> Teams = new Dictionary<uint, Team>();

        public uint Id { get; } = (uint) _s_rng.NextInt64(0, long.MaxValue);
        public readonly IList<uint> Members = new List<uint>();

        public Team()
        {
            Teams.Add(Id, this);
        }
    }
}
