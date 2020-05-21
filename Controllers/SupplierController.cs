using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using System.Linq;
using WidgetWebAPI.Domain;

namespace WidgetWebAPI.Controllers
{
	public class SupplierController : ODataController
	{
		private readonly DomainContext _context;

		public SupplierController(DomainContext context)
		{
			_context = context;
		}

		// Return all Suppliers
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Supplier> Get() => _context.Supplier.AsQueryable();

		// Return specific Supplier
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		public IQueryable<Supplier> Get([FromODataUri] int keySupplierId) => _context.Supplier.Where(e => e.SupplierId == keySupplierId);

		// Return List of Parts for Supplier
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
		[ODataRoute("Supplier({keySupplierId})/Part")]
		public IQueryable<SupplierPart> GetParts([FromODataUri] int keySupplierId) => _context.SupplierPart.Where(e => e.SupplierId == keySupplierId);

		// Return specific Part for supplier
		[EnableQuery(PageSize = 100, MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]

		// THIS DOESN'T WORK:
		//[ODataRoute("Supplier({keySupplierId})/Part({keyPartId})")]

		// Even though the OData specification says it should.  From https://docs.oasis-open.org/odata/odata/v4.01/os/part2-url-conventions/odata-v4.01-os-part2-url-conventions.docx
		// 4.3.3 URLs for Related Entities with Referential Constraints
		// If a navigation property leading to a related entity type has a partner navigation property that specifies a referential constraint, then those key properties of the related entity that take part in the referential constraint MAY be omitted from URLs.
		// Example 21: full key predicate of related entity
		// https://host/service/Orders(1)/Items(OrderID=1,ItemNo=2) 
		// Example 22: shortened key predicate of related entity
		// https://host/service/Orders(1)/Items(2) 
		// The two above examples are equivalent if the navigation property Items from Order to OrderItem has a partner navigation property from OrderItem to Order with a referential constraint tying the value of the OrderID key property of the OrderItem to the value of the ID key property of the Order.
		// The shorter form that does not specify the constrained key parts redundantly is preferred.If the value of the constrained key is redundantly specified, then it MUST match the principal key value.

		// BUT THIS DOES WORK:
		[ODataRoute("Supplier({keySupplierId})/Part({keySupplierId}, {keyPartId})")]
		// Lack of understanding of proper conventon or bug?
		public SingleResult<SupplierPart> GetPart([FromODataUri] int keySupplierId, [FromODataUri] int keyPartId)
			=> SingleResult.Create(_context.SupplierPart.Where(e => e.SupplierId == keySupplierId && e.PartId == keyPartId));
	}
}
