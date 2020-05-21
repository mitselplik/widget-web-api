using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using System.Linq;
using WidgetWebAPI.Domain;

namespace WidgetWebAPI.Controllers
{
	public class WidgetController : ODataController
	{
		private readonly DomainContext _context;

		public WidgetController(DomainContext context)
		{
			_context = context;
		}

		// Return all Widgets
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Widget> Get() => _context.Widget.AsQueryable();

		// Return specific Widget
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Widget> Get([FromODataUri] int keyWidgetId) => _context.Widget.Where(e => e.WidgetId == keyWidgetId);

		// Return List of Widget Parts
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		[ODataRoute("Widget({keyWidgetId})/Part")]
		public IQueryable<WidgetPart> GetParts([FromODataUri] int keyWidgetId) => _context.WidgetPart.Where(e => e.WidgetId == keyWidgetId);

		// Get List of a specific Part - there can be multiple, each with a distinct BlueprintId where the part is installed.

		// THIS DOESN'T WORK:
		// This kind of query would represent a partial key match.  The same can be achieved using a querystring though:
		// ~/odata/Widget(5)?$expand=Part($filter=Part eq 10)

		// I'd like to be able to have the above using this url instead:
		// ~/odata/Widget(5)/Part(10)

		// Even though this doesn't follow the OData Convention, it would be nice to be able to return a sub-set
		// of the child collection based on a partial key match.
		// But neither of these work since they represent a partial key.
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		// [ODataRoute("Widget({keyWidgetId})/Part({keyPartId})")]
		// [ODataRoute("Widget({keyWidgetId})/Part({keyWidgetId}, {keyPartId})")]

		// BUT THIS DOES WORK:
		// https://localhost:44316/odata/Widget(3)/Part(3,20,0)		-- Set of Items with this PartId
		// https://localhost:44316/odata/Widget(3)/Part(3,20,4)		-- This specific Part
		[ODataRoute("Widget({keyWidgetId})/Part({keyWidgetId}, {keyPartId}, {keyBlueprintId})")]
		public object GetPartList([FromODataUri] int keyWidgetId, [FromODataUri] int keyPartId, [FromODataUri] int keyBlueprintId = 0)
		{
			if (keyBlueprintId == 0)
			{
				return _context.WidgetPart.Where(e => e.WidgetId == keyWidgetId && e.PartId == keyPartId);
			}
			else
			{
				return _context.WidgetPart.FirstOrDefault(e => e.WidgetId == keyWidgetId && e.PartId == keyPartId && e.BlueprintId == keyBlueprintId);
			}
		}
	}
}
