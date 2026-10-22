using System;
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
            var postSlug = "comecando-com-ef-core";

            // 0. Limpa só os dados deste teste (Post primeiro, porque depende dos outros)
            context.Posts.RemoveRange(context.Posts.Where(x => x.Slug == postSlug).ToList());
            context.SaveChanges();
            context.Users.RemoveRange(context.Users.Where(x => x.Slug == "maria-silva-ef").ToList());
            context.Categories.RemoveRange(context.Categories.Where(x => x.Slug == "backend-ef").ToList());
            context.SaveChanges();

            // 1. CREATE: um Post que já traz autor e categoria novos
            var user = new User
            {
                Name = "Maria Silva",
                Slug = "maria-silva-ef",
                Email = "maria.ef@exemplo.com",
                Bio = "Desenvolvedora .NET",
                Image = "https://exemplo.com/maria.png",
                PasswordHash = "hash-de-exemplo"
            };
            var category = new Category { Name = "Backend", Slug = "backend-ef" };
            var post = new Post
            {
                Author = user,
                Category = category,
                Title = "Começando com EF Core",
                Summary = "Neste artigo vamos aprender EF Core",
                Body = "<p>Hello world</p>",
                Slug = postSlug,
                CreateDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };
            context.Posts.Add(post); // o EF também insere o autor e a categoria
            context.SaveChanges();
            Console.WriteLine($"[CREATE] Post \"{post.Title}\" criado (Id {post.Id})");

            // 2. READ com Include (traz autor e categoria junto)
            var posts = context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Slug == postSlug)
                .ToList();
            foreach (var item in posts)
                Console.WriteLine($"[READ] {item.Title} por {item.Author.Name} em {item.Category.Name}");

            // 3. UPDATE: precisa de tracking (sem AsNoTracking)
            var postToUpdate = context
                .Posts
                .Include(x => x.Author)
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Slug == postSlug);
            postToUpdate.Author.Name = "Joana Souza";
            context.Posts.Update(postToUpdate);
            context.SaveChanges();
            Console.WriteLine($"[UPDATE] Autor renomeado para: {postToUpdate.Author.Name}");

            // 4. READ final
            var updated = context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .First(x => x.Slug == postSlug);
            Console.WriteLine($"[READ] {updated.Title} por {updated.Author.Name} em {updated.Category.Name}");
        }
    }
}