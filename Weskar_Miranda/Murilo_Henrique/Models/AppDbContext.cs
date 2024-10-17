using Microsoft.EntityFrameworkCore;
using Murilo_Henrique.Models;

public class AppDbContext : DbContext
{
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<FolhaPagamento> FolhaPagamentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=weskar_miranda_e_murilo_henrique.db");
    }
}