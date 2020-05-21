using System.Collections.Generic;

namespace WidgetWebAPI.Data
{
	public partial class Part
	{
		public Part()
		{
			SupplierPartAssoc = new HashSet<SupplierPartAssoc>();
			WidgetPartAssoc = new HashSet<WidgetPartAssoc>();
		}

		public int PartId { get; set; }
		public string PartName { get; set; }
		public decimal Cost { get; set; }

		public virtual ICollection<SupplierPartAssoc> SupplierPartAssoc { get; set; }
		public virtual ICollection<WidgetPartAssoc> WidgetPartAssoc { get; set; }
	}
}
