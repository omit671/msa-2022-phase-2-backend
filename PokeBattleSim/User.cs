namespace PokeBattleSim
{
    /// <summary>
    /// A user / player / pokemon trainer.
    /// </summary>
    public class User
    {
        private static readonly Random _s_rng = new();

        public static readonly IDictionary<uint, User> Users = new Dictionary<uint, User>();

        public uint Id { get; } = (uint) _s_rng.NextInt64(0, long.MaxValue);
        public string Name { get; set; }
        public Team Team { get; set; } = new();

        public User(string name)
        {
            Name = name;

            Users.Add(Id, this);
        }
    }
}
