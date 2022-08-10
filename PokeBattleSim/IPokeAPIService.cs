namespace PokeBattleSim
{
    public interface IPokeAPIService
    {
        public Task<Pokemon> GetPokemon(string name);
    }
}
