using Microsoft.EntityFrameworkCore;
using System.Numerics;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<ItemList> itemLists { get; set; }
        public DbSet<User> users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var connect = builder.GetConnectionString("MyConnect");
            optionsBuilder.UseSqlServer(connect);
        }


    }
}
