namespace PokeBattleSim
{
    public class User
    {
        private static readonly Random _s_rng = new();

        public static readonly IDictionary<uint, User> Users = new Dictionary<uint, User>();

        public uint Id { get; }
        public string Name { get; set; }

        public User(string name)
        {
            Id = (uint) _s_rng.NextInt64(0, long.MaxValue);
            Name = name;

            Users.Add(Id, this);
        }
    }
}
