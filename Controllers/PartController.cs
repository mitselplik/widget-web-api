using Microsoft.AspNet.OData;
using System.Linq;
using WidgetWebAPI.Domain;

namespace WidgetWebAPI.Controllers
{
	public class PartController : ODataController
	{
		private readonly DomainContext _context;

		public PartController(DomainContext context)
		{
			_context = context;
		}

		// Return all Parts
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Part> Get() => _context.Part.AsQueryable();

		// Return specific Part
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Part> Get([FromODataUri] int keyPartId) => _context.Part.Where(e => e.PartId == keyPartId);



	}
}
