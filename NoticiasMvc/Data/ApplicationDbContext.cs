using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Models;

namespace NoticiasMvc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Noticia> Noticias => Set<Noticia>();
        public DbSet<NoticiaTag> NoticiasTags => Set<NoticiaTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabelas
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Tag>().ToTable("Tag");
            modelBuilder.Entity<Noticia>().ToTable("Noticia");
            modelBuilder.Entity<NoticiaTag>().ToTable("NoticiaTag");

            // Usuario
            modelBuilder.Entity<Usuario>(e =>
            {
                e.Property(p => p.Nome).IsRequired().HasMaxLength(250);
                e.Property(p => p.Senha).IsRequired().HasMaxLength(50);
                e.Property(p => p.Email).IsRequired().HasMaxLength(250);
                e.HasIndex(p => p.Email).IsUnique(false);
            });

            // Tag
            modelBuilder.Entity<Tag>(e =>
            {
                e.Property(p => p.Descricao).IsRequired().HasMaxLength(100);
                e.HasIndex(p => p.Descricao).IsUnique(); 
            });

            // Noticia
            modelBuilder.Entity<Noticia>(e =>
            {
                e.Property(p => p.Titulo).IsRequired().HasMaxLength(250);
                e.Property(p => p.Texto).IsRequired().HasColumnType("text");
                e.Property(p => p.UsuarioId).IsRequired();

                e.HasOne(p => p.Usuario)
                 .WithMany(u => u.Noticias!)
                 .HasForeignKey(p => p.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict); 
            });

            modelBuilder.Entity<NoticiaTag>(e =>
            {
                e.Property(p => p.NoticiaId).IsRequired();
                e.Property(p => p.TagId).IsRequired();

                e.HasOne(nt => nt.Noticia)
                 .WithMany(n => n.NoticiasTags!)
                 .HasForeignKey(nt => nt.NoticiaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(nt => nt.Tag)
                 .WithMany(t => t.NoticiasTags!)
                 .HasForeignKey(nt => nt.TagId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => new { p.NoticiaId, p.TagId }).IsUnique();
            });
        }
    }
}
