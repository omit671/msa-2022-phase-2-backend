namespace PokeBattleSim
{
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
            Pokemon? content = (Pokemon?)await result.Content.ReadFromJsonAsync(typeof(Pokemon));

            return content!;
        }
    }
}
