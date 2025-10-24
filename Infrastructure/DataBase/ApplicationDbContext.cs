using Back.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<ResultRequest> ResultRequests => Set<ResultRequest>();
        public DbSet<UserLike> UserLikes => Set<UserLike>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Подключаем все конфигурации из сборки Infrastructure
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
