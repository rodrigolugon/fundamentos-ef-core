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
            var slugs = new[] { "ef-dotnet", "ef-aspnet", "ef-aspnetcore" };

            using (var context = new BlogDataContext())
            {
                // 0. Limpa só as tags deste teste, para poder rodar várias vezes
                context.Tags.RemoveRange(context.Tags.Where(x => slugs.Contains(x.Slug)).ToList());
                context.SaveChanges();

                // 1. CREATE
                var tag = new Tag { Name = ".NET", Slug = "ef-dotnet" };
                var tag2 = new Tag { Name = "ASP.NET", Slug = "ef-aspnet" };
                context.Tags.Add(tag);
                context.Tags.Add(tag2);
                context.SaveChanges();
                Console.WriteLine($"[CREATE] Tags criadas: Id {tag.Id} (.NET) e Id {tag2.Id} (ASP.NET)");

                // 2. READ (AsNoTracking: só leitura, o EF não acompanha mudanças)
                var tags = context.Tags.AsNoTracking().Where(x => slugs.Contains(x.Slug)).ToList();
                Console.WriteLine($"[READ] {tags.Count} tag(s) do teste:");
                foreach (var item in tags)
                    Console.WriteLine($"  {item.Id} - {item.Name} ({item.Slug})");

                // 3. UPDATE (sem AsNoTracking: o EF precisa acompanhar a mudança)
                var tagToUpdate = context.Tags.FirstOrDefault(x => x.Id == tag2.Id);
                tagToUpdate.Name = "ASP.NET Core";
                tagToUpdate.Slug = "ef-aspnetcore";
                context.Update(tagToUpdate);
                context.SaveChanges();
                Console.WriteLine($"[UPDATE] Id {tagToUpdate.Id} agora é: {tagToUpdate.Name} ({tagToUpdate.Slug})");

                // 4. DELETE
                var tagToDelete = context.Tags.FirstOrDefault(x => x.Id == tag.Id);
                context.Remove(tagToDelete);
                context.SaveChanges();
                Console.WriteLine($"[DELETE] Tag Id {tag.Id} removida");

                // 5. READ final
                var remaining = context.Tags.AsNoTracking().Where(x => slugs.Contains(x.Slug)).ToList();
                Console.WriteLine($"[READ] {remaining.Count} tag(s) restante(s):");
                foreach (var item in remaining)
                    Console.WriteLine($"  {item.Id} - {item.Name} ({item.Slug})");
            }
        }
    }
}