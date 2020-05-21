namespace WidgetWebAPI.Domain
{
	public class SupplierPart
	{
		public int SupplierId { get; set; }
		public int PartId { get; set; }

		public string PartName { get; set; }
		public decimal Cost { get; set; }
		public decimal BuildLaborCost { get; set; }
		public decimal ReplacementLaborCost { get; set; }
		public decimal LastWholesalePrice { get; set; }
		public int LastOrderQuantity { get; set; }
	}
}
