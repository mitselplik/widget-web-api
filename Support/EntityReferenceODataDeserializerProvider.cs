using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using System;

namespace PacsNext.WebApi.Support
{
	/// <summary>
	/// Support class used to setup OData routing from this sample https://github.com/OData/WebApi/blob/master/samples/AspNetCore3xEndpointSample.Web/Startup.cs#L64
	/// based on this post to GitHub: https://github.com/OData/WebApi/issues/2164
	/// As near as I can tell, this does nothing more than provide a debugging breakpoint opportunity.
	/// </summary>
	class EntityReferenceODataDeserializerProvider : DefaultODataDeserializerProvider
	{
		public EntityReferenceODataDeserializerProvider(IServiceProvider rootContainer)
			: base(rootContainer)
		{

		}

		public override ODataEdmTypeDeserializer GetEdmTypeDeserializer(IEdmTypeReference edmType)
		{
			return base.GetEdmTypeDeserializer(edmType);
		}

		public override ODataDeserializer GetODataDeserializer(Type type, HttpRequest request)
		{
			return base.GetODataDeserializer(type, request);
		}
	}
}
