using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SignalRChat.Hubs;

using WebApp1.Data;

// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
// dotnet add package Microsoft.AspNetCore.Identity.UI

////////////

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppLoginDbContext>(opt => 
    opt.UseSqlite("Data Source=Users.db")
);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppLoginDbContext>();


builder.Services.AddSignalR();

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; //4
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
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub"); // 3

app.Run();
