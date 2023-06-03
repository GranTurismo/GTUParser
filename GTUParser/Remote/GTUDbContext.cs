using GTUParser.Models;
using Microsoft.EntityFrameworkCore;

namespace GTUParser.Remote;

public class GTUDbContext:DbContext
{
    public DbSet<Table> Tables { get; set; }
    public DbSet<Lecture> Lectures { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        string conString = "server=vps-4e996808.vps.ovh.net;database=Student;user=root;password=SNXnkPvTj9uj@";
        optionsBuilder.UseMySQL(conString);
    }
}