using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sqlServer.AddDatabase("database");

var webApi = builder.AddProject<WebApi>("webapi")
    .WithReference(db)
    .WaitFor(db);

builder.AddNpmApp("webapp", "../WebClient")
    .WithReference(webApi)
    .WaitFor(webApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();