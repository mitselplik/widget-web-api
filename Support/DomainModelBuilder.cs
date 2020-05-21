using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using System;

namespace WidgetWebAPI.Support
{
	class DomainModelBuilder : ODataConventionModelBuilder
	{
		#region Constructor(s)

		public DomainModelBuilder(IServiceProvider serviceProvider) : base(serviceProvider) { }

		#endregion

		public override IEdmModel GetEdmModel()
		{
			Namespace = "WidgetData";   // Hide Model Schema from $metadata

			EntityTypeConfiguration<Domain.Widget> WidgetConfiguration = EntitySet<Domain.Widget>("Widget").EntityType;
			WidgetConfiguration
				.HasKey(e => e.WidgetId)
				.Filter()   // Allow for the $filter Command
				.Count()    // Allow for the $count Command
				.Expand()   // Allow for the $expand Command
				.OrderBy()  // Allow for the $orderby Command
				.Page()     // Allow for the $top and $skip Commands
				.Select();  // Allow for the $select Command;
			WidgetConfiguration.HasMany(e => e.Part);

			EntityTypeConfiguration<Domain.WidgetPart> WidgetPartConfiguration = EntityType<Domain.WidgetPart>();
			WidgetPartConfiguration.HasKey(e => new { e.WidgetId, e.PartId, e.BlueprintId });

			EntityTypeConfiguration<Domain.Supplier> SupplierConfiguration = EntitySet<Domain.Supplier>("Supplier").EntityType;

			SupplierConfiguration
				.HasKey(e => e.SupplierId)
				.Filter()   // Allow for the $filter Command
				.Count()    // Allow for the $count Command
				.Expand()   // Allow for the $expand Command
				.OrderBy()  // Allow for the $orderby Command
				.Page()     // Allow for the $top and $skip Commands
				.Select();  // Allow for the $select Command;
			SupplierConfiguration.HasMany(e => e.Part);

			EntityTypeConfiguration<Domain.SupplierPart> SupplierPartConfiguration = EntityType<Domain.SupplierPart>();
			SupplierPartConfiguration.HasKey(e => new { e.SupplierId, e.PartId });

			EntityTypeConfiguration<Domain.Part> PartConfiguration = EntitySet<Domain.Part>("Part").EntityType;

			PartConfiguration
				.HasKey(e => e.PartId)
				.Filter()   // Allow for the $filter Command
				.Count()    // Allow for the $count Command
				.Expand()   // Allow for the $expand Command
				.OrderBy()  // Allow for the $orderby Command
				.Page()     // Allow for the $top and $skip Commands
				.Select();  // Allow for the $select Command;
			PartConfiguration.HasMany(e => e.Supplier);
			PartConfiguration.HasMany(e => e.Widget);

			return base.GetEdmModel();
		}
	}
}
