using System.Text.Json.Serialization;
using FastEndpoints;
using Scalar.AspNetCore;
using TagStudio.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Aspire
builder.AddServiceDefaults();

builder.AddWebServices();

builder.AddAppIdentityServices();
builder.AddTagsServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabasesAsync();

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


// ReSharper disable once ClassNeverInstantiated.Global

// Needed to access the Program class in tests
public partial class Program;