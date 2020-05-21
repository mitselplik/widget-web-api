using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WidgetWebAPI.Data;

namespace WidgetWebAPI.Domain
{
	public class DomainContext
	{
		#region Private Members

		private readonly WidgetDBContext _dbContext;
		private readonly IMapper _mapper;

		#endregion

		#region Constructor(s)

		public DomainContext(WidgetDBContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		#endregion

		public IQueryable<Widget> Widget => _dbContext.Widget.UseAsDataSource(_mapper).For<Widget>();
		public IQueryable<Part> Part => _dbContext.Widget.UseAsDataSource(_mapper).For<Part>();
		public IQueryable<Supplier> Supplier => _dbContext.Supplier.UseAsDataSource(_mapper).For<Supplier>();
		public IQueryable<WidgetPart> WidgetPart => _dbContext.WidgetPartAssoc.Include(e => e.Part).UseAsDataSource(_mapper).For<WidgetPart>();
		public IQueryable<SupplierPart> SupplierPart => _dbContext.SupplierPartAssoc.Include(e => e.Part).UseAsDataSource(_mapper).For<SupplierPart>();
	}
}
