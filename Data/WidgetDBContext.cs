using Microsoft.EntityFrameworkCore;

namespace WidgetWebAPI.Data
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

		public virtual DbSet<Part> Part { get; set; }
		public virtual DbSet<Supplier> Supplier { get; set; }
		public virtual DbSet<SupplierPartAssoc> SupplierPartAssoc { get; set; }
		public virtual DbSet<Widget> Widget { get; set; }
		public virtual DbSet<WidgetPartAssoc> WidgetPartAssoc { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Part>(entity =>
			{
				entity.ToTable("part");

				entity.HasIndex(e => e.PartName)
					.HasName("ndx_part_part_name")
					.IsUnique();

				entity.Property(e => e.PartId).HasColumnName("part_id");

				entity.Property(e => e.Cost)
					.HasColumnName("cost")
					.HasColumnType("decimal(8, 2)");

				entity.Property(e => e.PartName)
					.IsRequired()
					.HasColumnName("part_name")
					.HasMaxLength(100)
					.IsUnicode(false);
			});

			modelBuilder.Entity<Supplier>(entity =>
			{
				entity.ToTable("supplier");

				entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnName("name")
					.HasMaxLength(100)
					.IsUnicode(false);

				entity.Property(e => e.PreferredVendor).HasColumnName("preferred_vendor");
			});

			modelBuilder.Entity<SupplierPartAssoc>(entity =>
			{
				entity.HasKey(e => new { e.SupplierId, e.PartId })
					.HasName("pk_supplier_part_assoc");

				entity.ToTable("supplier_part_assoc");

				entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

				entity.Property(e => e.PartId).HasColumnName("part_id");

				entity.Property(e => e.LastOrderQuantity).HasColumnName("last_order_quantity");

				entity.Property(e => e.LastWholesalePrice)
					.HasColumnName("last_wholesale_price")
					.HasColumnType("numeric(8, 2)");

				entity.HasOne(d => d.Part)
					.WithMany(p => p.SupplierPartAssoc)
					.HasForeignKey(d => d.PartId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("fk_supplier_part_assoc_part");

				entity.HasOne(d => d.Supplier)
					.WithMany(p => p.SupplierPartAssoc)
					.HasForeignKey(d => d.SupplierId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("fk_supplier_part_assoc_supplier");
			});

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

			modelBuilder.Entity<WidgetPartAssoc>(entity =>
			{
				entity.HasKey(e => new { e.WidgetId, e.PartId, e.BlueprintId })
					.HasName("pk_widget_part_assoc");

				entity.ToTable("widget_part_assoc");

				entity.Property(e => e.WidgetId).HasColumnName("widget_id");

				entity.Property(e => e.PartId).HasColumnName("part_id");

				entity.Property(e => e.BlueprintId).HasColumnName("blueprint_id");

				entity.Property(e => e.BuildLaborCost)
					.HasColumnName("build_labor_cost")
					.HasColumnType("numeric(8, 2)");

				entity.Property(e => e.ReplacementLaborCost)
					.HasColumnName("replacement_labor_cost")
					.HasColumnType("numeric(8, 2)");

				entity.HasOne(d => d.Part)
					.WithMany(p => p.WidgetPartAssoc)
					.HasForeignKey(d => d.PartId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("fk_widget_part_assoc_part");

				entity.HasOne(d => d.Widget)
					.WithMany(p => p.WidgetPartAssoc)
					.HasForeignKey(d => d.WidgetId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("fk_widget_part_assoc_widget");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
