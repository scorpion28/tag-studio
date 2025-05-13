using System.Text.Json.Serialization;
using FastEndpoints;
using Scalar.AspNetCore;
using TagStudio.WebApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Aspire
builder.AddServiceDefaults();

builder.AddWebServices();
builder.AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();

    app.MapScalarApiReference(options =>
    {
        options.WithTagSorter(TagSorter.Alpha)
            .WithOperationSorter(OperationSorter.Alpha);
    });
    app.Map("/", () => Results.Redirect("/scalar/v1"));

    app.MapDefaultHealthCheckEndpoints();
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Endpoints.ApplyDefaultPolicies();

    c.Binding.BindUserIdFromClaims();

    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.Run();

// Needed to access the Program class in tests
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program;