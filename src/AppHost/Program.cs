using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sqlServer.AddDatabase("database");

builder.AddProject<WebApi>("webapi")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();