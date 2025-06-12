using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var db = sqlServer.AddDatabase("database");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(resourceBuilder =>
    {
        resourceBuilder.WithLifetime(ContainerLifetime.Persistent);
        resourceBuilder.WithDataVolume();
    });

var blob = storage.AddBlobs("blob");

var webApi = builder.AddProject<WebApi>("webapi")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(blob)
    .WaitFor(blob);

builder.AddNpmApp("webapp", "../WebClient")
    .WithReference(webApi)
    .WaitFor(webApi)
    .WithHttpEndpoint(targetPort: 5015, isProxied: false)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();