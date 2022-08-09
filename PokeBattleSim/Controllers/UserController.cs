using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PokeBattleSim.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly IDictionary<uint, string> s_users = new Dictionary<uint, string>();

        /// <summary>
        /// Retrieve all currently registered users.
        /// </summary>
        /// <response code="200">The user IDs of all registered users.</response>
        [HttpGet]
        public IEnumerable<uint> GetAllUsers()
        {
            return s_users.Keys;
        }

        /// <summary>
        /// Retrieve the name of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's name.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}/name")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserName(uint id)
        {
            string? name;

            bool userExists = s_users.TryGetValue(id, out name);

            if (userExists == false)
            {
                return NotFound();
            }
            else
            {
                return Ok(name);
            }
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <response code="201">The new user's ID.</response>
        /// <response code="403">If a user with the specified name already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(uint), 201)]
        [ProducesResponseType(403)]
        public IActionResult CreateUser([FromBody] string name)
        {
            if (s_users.Values.Contains(name))
            {
                return Forbid();
            }

            uint id = (uint)s_users.Count;

            s_users.Add(id, name);

            return Created($"api/v1/users/{id}", id);
        }
    }
}
