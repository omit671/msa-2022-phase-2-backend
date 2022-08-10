# MSA 2022 Phase 2 - Backend

*See also the [frontend repo](https://github.com/omit671/msa-2022-phase-2-frontend).*

# Running

Run `dotnet run --project PokeBattleSim --environment Development` to run the project in development mode.
This will enable debug logging (for both general loggers, and ASP.NET Core's loggers).

Run `dotnet run --project PokeBattleSim --environment Production` to run the project in release mode,
with build optimisations enabled. The logs will only show messages with a level of `warning` or higher.
In release mode, the swagger UI will not be available.