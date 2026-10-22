using System;
using Blog.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "A variável de ambiente DB_PASSWORD não foi configurada.");
            }

            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = "127.0.0.1,1433",
                InitialCatalog = "Blog",
                UserID = "sa",
                Password = password,
                TrustServerCertificate = true
            }.ConnectionString;

            options.UseSqlServer(connectionString);
        }
    }
}