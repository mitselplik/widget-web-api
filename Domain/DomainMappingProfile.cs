using AutoMapper;

namespace WidgetWebAPI.Domain
{
	public class DomainMappingProfile : Profile
	{
		public DomainMappingProfile()
		{
			CreateMap<Data.Widget, Domain.Widget>()
				.ForMember(e => e.Part, options => options.ExplicitExpansion())
				.ReverseMap();

			CreateMap<Data.Supplier, Domain.Supplier>()
				.ForMember(e => e.Part, options => options.ExplicitExpansion())
				.ReverseMap();

			CreateMap<Data.Part, Domain.Part>()
				.ForMember(e => e.Supplier, options => options.ExplicitExpansion())
				.ForMember(e => e.Widget, options => options.ExplicitExpansion())
				.ReverseMap();

			CreateMap<Data.WidgetPartAssoc, Domain.WidgetPart>()
				.IncludeMembers(dataModel => dataModel.Part)
				.ReverseMap();

			CreateMap<Data.SupplierPartAssoc, Domain.SupplierPart>()
				.IncludeMembers(dataModel => dataModel.Part)
				.ReverseMap();

			CreateMap<Data.Part, Domain.WidgetPart>(MemberList.None);
			CreateMap<Data.Part, Domain.SupplierPart>(MemberList.None);
		}
	}
}

