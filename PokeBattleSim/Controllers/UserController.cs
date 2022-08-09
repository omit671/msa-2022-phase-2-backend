using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PokeBattleSim.Controllers
{
    /// <summary>
    /// Create, manage, and retrieve users and their data.
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Retrieve all currently registered users.
        /// </summary>
        /// <response code="200">A list of all registered users.</response>
        [HttpGet]
        public IEnumerable<User> GetAllUsers()
        {
            return PokeBattleSim.User.Users.Values;
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <response code="201">The new user.</response>
        /// <response code="403">If a user with the specified name already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(403)]
        public IActionResult CreateUser([FromBody] string name)
        {
            if (PokeBattleSim.User.Users.Values.Any(user => user.Name == name))
            {
                return Forbid();
            }

            User user = new(name);

            return Created($"api/v1/users/{user.Id}", user);
        }

        /// <summary>
        /// Retrieve a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's data.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(uint id)
        {
            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out User? user);

            if (userExists == false)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">If the user was deleted successfully.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(uint id)
        {
            bool userExisted = PokeBattleSim.User.Users.Remove(id);

            if (userExisted == false)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Retrieve a user's name.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's name.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}/name")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserName(uint id)
        {
            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out User? user);

            if (userExists == false)
            {
                return NotFound();
            }

            return Ok(user!.Name);
        }

        /// <summary>
        /// Update a user's name.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="204">If the name change was processed successfully.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpPatch("{id}/name")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SetUserName(uint id, string name)
        {
            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out User? user);

            if (userExists == false)
            {
                return NotFound();
            }

            user!.Name = name;

            return NoContent();
        }

        /// <summary>
        /// Retrieve a user's team.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's team's ID.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}/team")]
        [ProducesResponseType(typeof(uint), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserTeam(uint id)
        {
            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out User? user);

            if (userExists == false)
            {
                return NotFound();
            }

            return Ok(user!.Team.Id);
        }

        /// <summary>
        /// Swap out a user's team.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="teamID">The team ID.</param>
        /// <response code="204">If the team change was processed successfully.</response>
        /// <response code="404">If the user or team specified does not exist.</response>
        [HttpPatch("{id}/team")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SetUserTeam(uint id, uint teamID)
        {
            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out User? user);

            if (userExists == false)
            {
                return NotFound();
            }

            bool teamExists = Team.Teams.TryGetValue(teamID, out Team? team);

            if (teamExists == false)
            {
                return NotFound();
            }

            user!.Team = team!;

            return NoContent();
        }
    }
}
