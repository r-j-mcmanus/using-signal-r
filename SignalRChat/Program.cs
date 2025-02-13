using SignalRChat.Hubs; // 1

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(); // 2

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
}); // 4


var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins); // 5

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
