﻿using Microsoft.AspNetCore.Mvc;

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
        /// <response code="200">The user IDs of all registered users.</response>
        [HttpGet]
        public IEnumerable<uint> GetAllUsers()
        {
            return PokeBattleSim.User.Users.Keys;
        }

        /// <summary>
        /// Retrieve the name of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's name.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(uint id)
        {
            User? user;

            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out user);

            if (userExists == false)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        /// <summary>
        /// Retrieve a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">The user's name.</response>
        /// <response code="404">If the user specified does not exist.</response>
        [HttpGet("{id}/name")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserName(uint id)
        {
            User? user;

            bool userExists = PokeBattleSim.User.Users.TryGetValue(id, out user);

            if (userExists == false)
            {
                return NotFound();
            }
            else
            {
                return Ok(user.Name);
            }
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
    }
}