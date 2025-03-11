using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp1.Data;

public class AppLoginDbContext : IdentityDbContext
{
    public AppLoginDbContext(DbContextOptions<AppLoginDbContext> options)
        : base(options)
    {
    }
}
