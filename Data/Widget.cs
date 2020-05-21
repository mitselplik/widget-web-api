using System.Collections.Generic;

namespace WidgetWebAPI.Data
{
	public partial class Widget
	{
		public Widget()
		{
			WidgetPartAssoc = new HashSet<WidgetPartAssoc>();
		}

		public int WidgetId { get; set; }
		public string WidgetName { get; set; }

		public virtual ICollection<WidgetPartAssoc> WidgetPartAssoc { get; set; }
	}
}
