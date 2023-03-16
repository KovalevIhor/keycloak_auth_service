using System.Net;
using System.Security.Claims;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Web.Swagger;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services
  .AddEndpointsApiExplorer()
  .AddSwagger();

services.AddKeycloakAuthentication(configuration);

services
  .AddAuthorization()
  // .AddKeycloakAuthorization(configuration)
  ;

services.AddKeycloakAdminHttpClient(configuration);

var app = builder.Build();

app
  .UseSwagger()
  .UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

const string realm = "master";

app.MapGet("/auth", (ClaimsPrincipal user) =>
{
  app.Logger.LogInformation(user.Identity?.Name);
}).RequireAuthorization();

app.MapGet("/user-list", async (IKeycloakUserClient client) => Results.Json(await client.GetUsers(realm)));

app.MapGet("/user-create", async (IKeycloakUserClient client) =>
{
  var username = $"user_name_{Guid.NewGuid()}";
  var userBody = new User
  {
    Username = username,
    Email = $"{username}@example.com",
    Enabled = true,
    Groups = new []{ "naps_admin" },
    Attributes = new Dictionary<string, string[]>
    {
      {
        "fleet_id", new []{ "1" }
      }
    },
  };

  var createdUserResult = await client.CreateUser(realm, userBody);
  if (createdUserResult.StatusCode != HttpStatusCode.Created)
  {
    Results.StatusCode(400);
  }

  var users = await client.GetUsers(realm, new GetUsersRequestParameters { Username = username });
  var createdUser = users.FirstOrDefault();

  return Results.Created("", createdUser);
});

app.Run();
