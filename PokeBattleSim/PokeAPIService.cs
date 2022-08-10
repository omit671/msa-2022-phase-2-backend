namespace PokeBattleSim
{
    /// <summary>
    /// This Typed client provides a typed interface for
    /// calling the PokeAPI, and handles the deserialisation
    /// of responses.
    /// 
    /// It is created by the Swagger middleware, and passed
    /// to the TeamController via dependency injection.
    /// </summary>
    public class PokeAPIService : IPokeAPIService
    {
        private readonly HttpClient _httpClient;

        public PokeAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        }

        public async Task<Pokemon> GetPokemon(string name)
        {
            var result = await _httpClient.GetAsync("pokemon/" + name.ToLower());

            Pokemon? content = (Pokemon?) await result.Content.ReadFromJsonAsync(typeof(Pokemon));

            return content!;
        }
    }
}
