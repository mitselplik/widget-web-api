using System.Collections.Generic;

namespace WidgetWebAPI.Domain
{
	public class Widget
	{
		public int WidgetId { get; set; }
		public string WidgetName { get; set; }

		public ICollection<WidgetPart> Part { get; set; }
	}
}
