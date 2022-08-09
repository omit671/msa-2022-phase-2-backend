using Microsoft.AspNetCore.Mvc;

namespace PokeBattleSim.Controllers
{
    /// <summary>
    /// Manage teams of Pokemon.
    /// </summary>
    [Route("api/v1/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly HttpClient _client;

        public TeamController(IHttpClientFactory clientFactory)
        {
            ArgumentNullException.ThrowIfNull(clientFactory, nameof(clientFactory));

            _client = clientFactory.CreateClient("PokeAPI");
        }

        /// <summary>
        /// Retrieve all currently existent Pokemon teams.
        /// </summary>
        /// <response code="200">The team IDs of all registered teams.</response>
        [HttpGet]
        public IEnumerable<uint> GetAllTeams()
        {
            return Team.Teams.Keys;
        }

        /// <summary>
        /// Create a new team, optionally from a list of pokemon.
        /// </summary>
        /// <param name="pokemon">A list of initial team members. Optional.</param>
        /// <response code="201">The new team.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Team), 201)]
        public IActionResult CreateTeam([FromBody] IList<string>? pokemon)
        {
            Team team = new();

            if (pokemon != null)
            {
                // TODO populate team members
            }

            return Created($"api/v1/teams/{team.Id}", team);
        }

        /// <summary>
        /// Retrieve a Pokemon team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <response code="200">The team's data.</response>
        /// <response code="404">If the team specified does not exist.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetTeam(uint id)
        {
            bool teamExists = Team.Teams.TryGetValue(id, out Team? team);

            if (teamExists == false)
            {
                return NotFound();
            }

            return Ok(team);
        }

        /// <summary>
        /// Delete a Pokemon team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <response code="200">If the team was deleted successfully.</response>
        /// <response code="404">If the team specified does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Team), 204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTeam(uint id)
        {
            bool teamExisted = Team.Teams.Remove(id);

            if (teamExisted == false)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Get the team members of a Pokemon team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <response code="200">A list of Pokemon IDs.</response>
        /// <response code="404">If the team specified does not exist.</response>
        [HttpGet("{id}/members")]
        [ProducesResponseType(typeof(IEnumerable<uint>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetMembers(uint id)
        {
            bool teamExists = Team.Teams.TryGetValue(id, out Team? team);

            if (teamExists == false)
            {
                return NotFound();
            }

            return Ok(team!.Members);
        }

        /// <summary>
        /// Get the team members of a Pokemon team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <param name="members">A list of Pokemon names.</param>
        /// <response code="204"></response>
        /// <response code="404">If the team specified does not exist.</response>
        [HttpPut("{id}/members")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutMembers(uint id, [FromBody] List<string> members)
        {
            bool teamExists = Team.Teams.TryGetValue(id, out Team? team);

            if (teamExists == false)
            {
                return NotFound();
            }

            team!.Members.Clear();

            foreach (string member in members)
            {
                team.Members.Add(await _getPokemonID(member));
            }

            return NoContent();
        }

        private record Pokemon(uint id, string name);

        private async Task<uint> _getPokemonID(string name)
        {
            var result = await _client.GetAsync("pokemon/" + name.ToLower());
            Pokemon? content = (Pokemon?) await result.Content.ReadFromJsonAsync(typeof(Pokemon));

            return content!.id;
        }
    }
}
