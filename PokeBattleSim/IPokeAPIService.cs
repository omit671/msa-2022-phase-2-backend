namespace PokeBattleSim
{
    /// <summary>
    /// Here we create an interface for a Typed Client.
    /// This interface gives our controller code type-safety
    /// when making calls to the PokeAPI. It also handles
    /// deserialisation of the response, so caller codes does
    /// not need to duplicate it.
    /// 
    /// It is an interface so we can mock it in our testing
    /// code, to avoid making network calls during our tests.
    /// </summary>
    /// <see cref="PokeAPIService"/>
    public interface IPokeAPIService
    {
        public Task<Pokemon> GetPokemon(string name);
    }
}
