namespace WidgetWebAPI.Domain
{
	public class WidgetPart
	{
		public int WidgetId { get; set; }
		public int PartId { get; set; }
		public int BlueprintId { get; set; }

		public string PartName { get; set; }
		public decimal Cost { get; set; }
		public decimal BuildLaborCost { get; set; }
		public decimal ReplacementLaborCost { get; set; }
	}
}
