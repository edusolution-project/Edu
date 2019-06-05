using CoreEDB.CoreModels;
using Microsoft.EntityFrameworkCore;

namespace CoreEDB.Database
{
    public class ExtendsContext<TEntity> : DbContext where TEntity : EntityBase
    {
        protected readonly string _tableName;
        public ExtendsContext(string tableName) : base()
        {
            _tableName = tableName;
        }
        public virtual DbSet<TEntity> GetEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Database.SetInitializer<ExtendsContext<TEntity>>(null);
            modelBuilder.Entity<TEntity>().ToTable(_tableName);
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(StaticGetConfig.StrConnecting);
            }
        }
    }
}
