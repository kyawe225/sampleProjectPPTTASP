using BookingAppllicaiton.Tables;
using Microsoft.EntityFrameworkCore;

namespace BookingAppllicaiton.Context;

public class DatabaseContext:DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    public DbSet<User> User { set; get; }
    public DbSet<PackageTable> Package { set; get; }
    public DbSet<PackageUser> PackageUsers { set; get; }
    public DbSet<ClassTable> Classes { set; get; }
    public DbSet<Schedule> Schedules { set; get; }
    
}