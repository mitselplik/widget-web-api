namespace WidgetWebAPI.Data
{
	public partial class SupplierPartAssoc
	{
		public int SupplierId { get; set; }
		public int PartId { get; set; }
		public decimal LastWholesalePrice { get; set; }
		public int LastOrderQuantity { get; set; }

		public virtual Part Part { get; set; }
		public virtual Supplier Supplier { get; set; }
	}
}
