using System.Collections.Generic;

namespace WidgetWebAPI.Domain
{
	public class Part
	{
		public int PartId { get; set; }
		public string PartName { get; set; }
		public decimal Cost { get; set; }

		public ICollection<SupplierPart> Supplier { get; set; }
		public ICollection<WidgetPart> Widget { get; set; }
	}
}
