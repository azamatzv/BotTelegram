using BotTelegram.Models;
using Microsoft.EntityFrameworkCore;

namespace BotTelegram.Data;

public class BotContext : DbContext
{
    public DbSet<InfoUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=0932;Database=TGbot");
    }
}