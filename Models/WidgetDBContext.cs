using Microsoft.EntityFrameworkCore;

namespace WidgetWebAPI.Models
{
    public partial class WidgetDBContext : DbContext
    {
        public WidgetDBContext()
        {
        }

        public WidgetDBContext(DbContextOptions<WidgetDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Widget> Widget { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=.;Database=WidgetDB;Trusted_Connection=True;");
                
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Widget>(entity =>
            {
                entity.ToTable("widget");

                entity.Property(e => e.WidgetId).HasColumnName("widget_id");

                entity.Property(e => e.WidgetName)
                    .IsRequired()
                    .HasColumnName("widget_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
