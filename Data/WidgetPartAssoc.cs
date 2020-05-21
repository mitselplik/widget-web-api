namespace WidgetWebAPI.Data
{
	public partial class WidgetPartAssoc
	{
		public int WidgetId { get; set; }
		public int PartId { get; set; }
		public int BlueprintId { get; set; }
		public decimal BuildLaborCost { get; set; }
		public decimal ReplacementLaborCost { get; set; }

		public virtual Part Part { get; set; }
		public virtual Widget Widget { get; set; }
	}
}
