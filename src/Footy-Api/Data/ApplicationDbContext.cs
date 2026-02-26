using Footy_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Footy_Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();
}