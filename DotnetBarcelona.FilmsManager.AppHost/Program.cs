var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var mssql = builder.AddSqlServer("films-manager-sql", port: 65379).WithLifetime(ContainerLifetime.Persistent);

var actorsDb = mssql.AddDatabase("ActorsDb");
var filmsDb = mssql.AddDatabase("FilmsDb");

var actorsApi = builder.AddProject<Projects.DotnetBarcelona_Actors_WebAPI>("actors-webapi")
    .WithReference(actorsDb)
    .WaitFor(actorsDb);

var filmsApi = builder.AddProject<Projects.DotnetBarcelona_Films_WebAPI>("films-webapi")
    .WithReference(filmsDb)
    .WaitFor(filmsDb);

var apiService = builder.AddProject<Projects.DotnetBarcelona_FilmsManager_WebAPI>("webapi")
    .WithEnvironment("ConnectionStrings__FilmsApiUrl", filmsApi.GetEndpoint("https"))
    .WithEnvironment("ConnectionStrings__ActorsApiUrl", actorsApi.GetEndpoint("https"));

builder.AddProject<Projects.DotnetBarcelona_FilmsManager_WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();