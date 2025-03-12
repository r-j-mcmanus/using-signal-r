using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using SignalRChat.Hubs;
using SignalRChat.Data;

// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
// dotnet add package Microsoft.AspNetCore.Identity.UI
// dotnet add package Microsoft.IdentityModel.Tokens
// dotnet add package NSwag.AspNetCore
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


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
}); 
builder.Services.AddIdentityApiEndpoints<IdentityUser>() // registers a set of endpoints. IdentityUser is the ~default~ user model provided by ASP.NET Identity
    .AddEntityFrameworkStores<AuthDbContext>(); // store user data in a database

// for webhooks
builder.Services.AddSignalR();

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
