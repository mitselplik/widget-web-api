using System.Collections.Generic;

namespace WidgetWebAPI.Domain
{
	public class Supplier
	{
		public int SupplierId { get; set; }
		public string Name { get; set; }
		public bool PreferredVendor { get; set; }

		public ICollection<SupplierPart> Part { get; set; }
	}
}
