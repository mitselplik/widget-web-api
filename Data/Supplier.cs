using System.Collections.Generic;

namespace WidgetWebAPI.Data
{
	public partial class Supplier
	{
		public Supplier()
		{
			SupplierPartAssoc = new HashSet<SupplierPartAssoc>();
		}

		public int SupplierId { get; set; }
		public string Name { get; set; }
		public bool PreferredVendor { get; set; }

		public virtual ICollection<SupplierPartAssoc> SupplierPartAssoc { get; set; }
	}
}
