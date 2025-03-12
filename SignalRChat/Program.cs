using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;

using SignalRChat.Hubs;
using SignalRChat.Data;
using SignalRChat.Providers;

// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
// dotnet add package Microsoft.AspNetCore.Identity.UI
// dotnet add package Microsoft.IdentityModel.Tokens
// dotnet add package NSwag.AspNetCore
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
// dotnet add package Microsoft.EntityFrameworkCore.InMemory
// dotnet add package Microsoft.EntityFrameworkCore.Design

// dotnet ef migrations add InitialCreate
// dotnet ef database update



// https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-9.0

////////////

var builder = WebApplication.CreateBuilder(args);

// so our app can use mysql
builder.Services.AddDbContext<AuthDbContext>(opt => 
    opt.UseSqlite("Data Source=Users.db")
);



// for identity 
builder.Services.AddAuthentication(options => // sets up the authentication system in ASP.NET Core
{
    // Identity made Cookie authentication the default.
    // However, we want JWT Bearer Auth to be the default.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
  {
      // Configure the Authority to the expected value for
      // the authentication provider. This ensures the token
      // is appropriately validated.
      // options.Authority = "Authority URL"; // TODO: Update URL

      // We have to hook the OnMessageReceived event in order to
      // allow the JWT authentication handler to read the access
      // token from the query string when a WebSocket or 
      // Server-Sent Events request comes in.

      // Sending the access token in the query string is required when 
      // using WebSockets or ServerSentEvents due to a limitation in 
      // Browser APIs. We restrict it to only calls to the
      // SignalR hub in this code.
      // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
      // for more information about security considerations when using
      // the query string to transmit the access token.
      // Query string -> part of the url, NOT the header sent
      options.Authority = "https://localhost:5235"; // Ensure this matches your Identity URL

      options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              Console.WriteLine("Here");
              var accessToken = context.Request.Query["access_token"];

              // If the request is for our hub...
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) &&
                  (path.StartsWithSegments("/chathub")))
              {
                  // Read the token out of the query string
                  context.Token = accessToken;
              }
              return Task.CompletedTask;
          }
      };
  });
builder.Services.AddIdentityApiEndpoints<IdentityUser>() // registers a set of endpoints. IdentityUser is the ~default~ user model provided by ASP.NET Identity
    .AddEntityFrameworkStores<AuthDbContext>(); // store user data in a database

// for webhooks
builder.Services.AddSignalR();
// IUserIdProvider from signalR
// NameUserIdProvider a dto
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

// for swagger
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

// for cors
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5500")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});


////////////

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "TodoAPI";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
});

app.UseHttpsRedirection();

// the following three MUST be in this order to work
app.UseRouting();
app.UseAuthentication(); // Identity is enabled by calling UseAuthentication. adds authentication middleware
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapHub<ChatHub>("/chatHub");

app.Run();
