using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            using var context = new BlogDataContext();

            // 0. Limpa os dados (as tabelas de ligação são limpas em cascata pelo banco)
            context.Posts.RemoveRange(context.Posts.ToList());
            context.SaveChanges();
            context.Users.RemoveRange(context.Users.ToList());
            context.Categories.RemoveRange(context.Categories.ToList());
            context.Tags.RemoveRange(context.Tags.ToList());
            context.Roles.RemoveRange(context.Roles.ToList());
            context.SaveChanges();

            // 1. CREATE: um grafo completo (post, autor com papel, categoria e duas tags)
            var role = new Role { Name = "Autor", Slug = "autor" };
            var user = new User
            {
                Name = "Maria Silva",
                Slug = "maria-silva",
                Email = "maria@exemplo.com",
                Bio = "Desenvolvedora .NET",
                Image = "https://exemplo.com/maria.png",
                PasswordHash = "hash-de-exemplo",
                Roles = new List<Role> { role }
            };
            var category = new Category { Name = "Backend", Slug = "backend" };
            var tagDotnet = new Tag { Name = ".NET", Slug = "dotnet" };
            var tagEf = new Tag { Name = "EF Core", Slug = "efcore" };

            var post = new Post
            {
                Author = user,
                Category = category,
                Title = "Começando com EF Core",
                Summary = "Neste artigo vamos aprender EF Core",
                Body = "<p>Hello world</p>",
                Slug = "comecando-com-ef-core",
                CreateDate = DateTime.Now,
                LastUpdateDate = DateTime.Now,
                Tags = new List<Tag> { tagDotnet, tagEf }
            };

            context.Posts.Add(post);
            context.SaveChanges();
            Console.WriteLine("[SEED] Dados criados: 1 post, 1 autor, 1 papel, 1 categoria e 2 tags");

            // 2. READ: carrega tudo com Include / ThenInclude
            var posts = context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .ToList();

            foreach (var item in posts)
            {
                var roles = string.Join(", ", item.Author.Roles.Select(r => r.Name));
                var tags = string.Join(", ", item.Tags.Select(t => t.Name));
                Console.WriteLine($"[READ] {item.Title} por {item.Author.Name} ({roles}) em {item.Category.Name}");
                Console.WriteLine($"       Tags: {tags}");
            }
        }
    }
}