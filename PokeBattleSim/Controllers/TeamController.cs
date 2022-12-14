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
        private readonly IPokeAPIService _pokeAPIService;
        
        /*
         * Here we use dependency injection to have the Swashbuckle middleware
         * pass in a service for querying our 3rd party API. It simplifies our
         * code because we don't have to worry about making HTTP requests,
         * constructing the URL to query, or deserialising the response. It also
         * lets us swap out the implementation easily, if we need to. This
         * increases the testability of the code, because it lets us easily pass
         * in a mock service. With a mock, we can check if our code is calling
         * the right API methods at the right times.
         */
        public TeamController(IPokeAPIService pokeAPIService)
        {
            ArgumentNullException.ThrowIfNull(pokeAPIService, nameof(pokeAPIService));

            _pokeAPIService = pokeAPIService;
        }

        /// <summary>
        /// Retrieve all currently existent Pokemon teams.
        /// </summary>
        /// <response code="200">All registered teams.</response>
        [HttpGet]
        public IEnumerable<Team> GetAllTeams()
        {
            return Team.Teams.Values;
        }

        /// <summary>
        /// Create a new team, optionally from a list of pokemon.
        /// </summary>
        /// <param name="pokemon">A list of initial team members. May be empty.</param>
        /// <response code="201">The new team.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Team), 201)]
        public async Task<IActionResult> CreateTeam([FromBody] IList<string>? pokemon)
        {
            Team team = new();

            if (pokemon != null)
            {
                foreach (string member in pokemon)
                {
                    team.Members.Add(await _pokeAPIService.GetPokemon(member));
                }
            }

            return Created($"/api/v1/teams/{team.Id}", team);
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
        /// <response code="200">A list of Pokemon.</response>
        /// <response code="404">If the team specified does not exist.</response>
        [HttpGet("{id}/members")]
        [ProducesResponseType(typeof(IEnumerable<Pokemon>), 200)]
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
                team.Members.Add(await _pokeAPIService.GetPokemon(member));
            }

            return NoContent();
        }
    }
}
