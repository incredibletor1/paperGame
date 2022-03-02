using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaperGame.Models.DAL;
using PaperGame.Models.Context.Interface;

namespace PaperGame.Models.Context
{
    public class PaperGameContext : DbContext, IPaperGameContext
    {
        public PaperGameContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<GameRoom> GameRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
            .Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

            builder
            .Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            builder
            .Entity<GameRoom>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        }
    }
}
