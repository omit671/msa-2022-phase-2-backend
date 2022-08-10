using Microsoft.AspNetCore.Mvc;
using PokeBattleSim;
using PokeBattleSim.Controllers;

namespace PokeBattleSimTest
{
    public class TeamControllerTest
    {
        [SetUp]
        public void Setup()
        {
            User.Users.Clear();
            Team.Teams.Clear();
        }

        /*
         * Here we create a substitute PokeAPIService that doesn't make any network
         * requests. The use of NSubstitute allows us to easily create a mock object,
         * so we can test only the code of the TeamController and not PokeAPIService,
         * HttpClient, etc. This means we can perform unit tests on just the TeamController,
         * and aren't forced to use integration tests.
         */
        private IPokeAPIService createPokeAPIService()
        {
            var substitute = Substitute.For<IPokeAPIService>();

            uint pokemonCreated = 0;

            substitute.GetPokemon(default).ReturnsForAnyArgs(callInfo => new Pokemon(pokemonCreated++, callInfo.ArgAt<string>(0)));

            return substitute;
        }

        /// <summary>
        /// Check that the teams/ endpoint returns all registered
        /// teams (and nothing else).
        /// </summary>
        [Test]
        public void TestTeamListing()
        {
            var pokeApiService = createPokeAPIService();
            TeamController teamController = new(pokeApiService);

            Assert.That(teamController.GetAllTeams().SequenceEqual(new List<Team> { }));

            Team team1 = new();

            Assert.That(teamController.GetAllTeams().SequenceEqual(new List<Team> { team1 }));

            Team team2 = new();

            Assert.That(teamController.GetAllTeams().SequenceEqual(new List<Team> { team1, team2 }));

            Team.Teams.Clear();

            Assert.That(teamController.GetAllTeams().SequenceEqual(new List<Team>()));
        }

        /// <summary>
        /// Check that teams are created and registered succesfully.
        /// Also check that any Pokemon members are converted from
        /// names to IDs using PokeAPI.
        /// </summary>
        [Test]
        public void TestTeamCreation()
        {
            var pokeApiService = createPokeAPIService();
            TeamController teamController = new(pokeApiService);

            Assert.That(Team.Teams, Is.EqualTo(new List<Team> { }));

            // First call

            Task<IActionResult> asyncResult = teamController.CreateTeam(new List<string> { });
            asyncResult.Wait();
            IActionResult result = asyncResult.Result;

            Team team1 = Team.Teams.Values.First();

            Assert.IsInstanceOf(typeof(CreatedResult), result);

            CreatedResult createdResult = (CreatedResult) result;

            Assert.That(createdResult.Location, Is.EqualTo($"/api/v1/teams/{team1.Id}"));
            Assert.That(createdResult.Value, Is.EqualTo(team1));

            // Check that the API wasn't called
            Assert.That(team1.Members, Is.EqualTo(new List<Pokemon> { }));
            pokeApiService.DidNotReceive().GetPokemon(default);

            // Second call

            asyncResult = teamController.CreateTeam(new List<string> { "pikachu" });
            asyncResult.Wait();
            result = asyncResult.Result;

            Team team2 = Team.Teams.Values.Where(team => team != team1).First();

            Assert.IsInstanceOf(typeof(CreatedResult), result);

            createdResult = (CreatedResult) result;

            Assert.That(createdResult.Location, Is.EqualTo($"/api/v1/teams/{team2.Id}"));
            Assert.That(createdResult.Value, Is.EqualTo(team2));

            // Check API was called
            Assert.That(team2.Members, Is.EqualTo(new List<Pokemon> { new Pokemon(0, "pikachu") }));
            pokeApiService.Received().GetPokemon("pikachu");
        }

        /// <summary>
        /// Check teams can be retrieved by their ID.
        /// </summary>
        [Test]
        public void TestTeamRetrieval()
        {
            var pokeApiService = createPokeAPIService();
            TeamController teamController = new(pokeApiService);

            Team team1 = new();
            Team team2 = new();
            Team team3 = new();

            // Check team 2

            IActionResult result = teamController.GetTeam(team2.Id);
            Assert.IsInstanceOf<OkObjectResult>(result);

            OkObjectResult okResult = (OkObjectResult) result;
            Assert.That(okResult.Value, Is.EqualTo(team2));

            // Check team 1

            result = teamController.GetTeam(team1.Id);
            Assert.IsInstanceOf<OkObjectResult>(result);

            okResult = (OkObjectResult) result;
            Assert.That(okResult.Value, Is.EqualTo(team1));

            // Check team 3

            result = teamController.GetTeam(team3.Id);
            Assert.IsInstanceOf<OkObjectResult>(result);

            okResult = (OkObjectResult) result;
            Assert.That(okResult.Value, Is.EqualTo(team3));

            // Check non-existent team

            result = teamController.GetTeam(0);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// Check that deleted teams are deregistered.
        /// </summary>
        [Test]
        public void TestTeamDeletion()
        {
            var pokeApiService = createPokeAPIService();
            TeamController teamController = new(pokeApiService);

            Team team1 = new();
            Team team2 = new();
            Team team3 = new();

            var expected = new List<Team> { team1, team2, team3 };
            var actual = Team.Teams.Values;

            // Check they're the same set-wise
            Assert.True(expected.Except(actual).Any() == false && actual.Except(expected).Any() == false);

            // Bye-bye team 2
            teamController.DeleteTeam(team2.Id);

            expected = new List<Team> { team1, team3 };
            actual = Team.Teams.Values;

            // Check they're the same set-wise
            Assert.True(expected.Except(actual).Any() == false && actual.Except(expected).Any() == false);
        }
    }
}