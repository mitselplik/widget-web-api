using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PacsNext.WebApi.Support
{
	/// <summary>
	/// An ASP.Net Core version of the custom routing convention class described here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/odata-support-in-aspnet-web-api/odata-routing-conventions#custom-routing-conventions
	/// This class essentially allows URLs like the following to work: /odata/Products({keyParentId})/Suppliers({childSubKeyId})
	/// </summary>
	class NavigationIndexRoutingConvention : IODataRoutingConvention
	{
		public IEnumerable<ControllerActionDescriptor> SelectAction(RouteContext routeContext)
		{
			// Get a IActionDescriptorCollectionProvider from the global service provider
			IActionDescriptorCollectionProvider actionCollectionProvider =
				routeContext.HttpContext.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();
			Contract.Assert(actionCollectionProvider != null);

			// Get OData path from HttpContext
			Microsoft.AspNet.OData.Routing.ODataPath odataPath = routeContext.HttpContext.ODataFeature().Path;
			HttpRequest request = routeContext.HttpContext.Request;

			if (request.Method == "GET" && odataPath.PathTemplate.Equals("~/entityset/key"))
			{
				string controllerName = odataPath.Segments[0].Identifier;
				// Get the list of action descriptors for this controller
				var actionDescriptors = actionCollectionProvider.ActionDescriptors.Items
					.Cast<ControllerActionDescriptor>()
					.Where(ad => ad.ControllerName == controllerName && ad.ActionName == "Get");

				foreach (var actionDescriptor in actionDescriptors.OrderByDescending(item => item.ActionName.Length))
				{
					// We are only interested in examining actions with two parameters.
					if (actionDescriptor.Parameters.Count != 1) continue;

					// See if an OdataRouteAttribute exists in the EndpointMetaData
					ODataRouteAttribute oDataRouteAttribute = actionDescriptor.EndpointMetadata.OfType<ODataRouteAttribute>().FirstOrDefault();
					string actionName = actionDescriptor.ActionName;
					Microsoft.OData.UriParser.KeySegment keyFirst = odataPath.Segments[1] as Microsoft.OData.UriParser.KeySegment;
					object keyParentId = keyFirst.Keys.ToList()[0].Value;
					routeContext.RouteData.Values["odataPath"] = odataPath;
					routeContext.RouteData.Values[actionDescriptor.Parameters[0].Name] = keyParentId;
					routeContext.RouteData.Values[ODataRouteConstants.Key] = keyParentId;
					return new[] { actionDescriptor };
				}
			}
			else if (request.Method == "GET" && odataPath.PathTemplate.Equals("~/entityset/key/unresolved"))
			{
				string controllerName = odataPath.Segments[0].Identifier;
				// Get the list of action descriptors for this controller
				var actionDescriptors = actionCollectionProvider.ActionDescriptors.Items
					.Cast<ControllerActionDescriptor>()
					.Where(ad => ad.ControllerName == controllerName);

				Microsoft.OData.UriParser.KeySegment keyFirst = odataPath.Segments[1] as Microsoft.OData.UriParser.KeySegment;
				object keyParentId = keyFirst.Keys.ToList()[0].Value;

				string unresolvedPattern = odataPath.Segments[2].ToString();
				// Go from longest named action to shortest to ensure that we don't confuse which action to
				// call if two actions have similar names such that one is a sub-set of the other.
				foreach (var actionDescriptor in actionDescriptors.OrderByDescending(item => item.ActionName.Length))
				{
					// We are only interested in examining actions with two parameters.
					if (actionDescriptor.Parameters.Count != 2) continue;

					// See if an OdataRouteAttribute exists in the EndpointMetaData
					ODataRouteAttribute oDataRouteAttribute = actionDescriptor.EndpointMetadata.OfType<ODataRouteAttribute>().FirstOrDefault();
					string actionName = actionDescriptor.ActionName;
					if (oDataRouteAttribute != null)
					{
						string pathTemplate = oDataRouteAttribute.PathTemplate;
						// The only template we want to match is in the shape "{ControllerName}({Parameter 1 Name})/{FunctionName}({Parameter 2 Name})
						// If the pattern doesn't match that, then return null;
						string compareStartPath = string.Format("{0}({{{1}}})/", controllerName, actionDescriptor.Parameters[0].Name);
						string compareEndPath = string.Format("({{{0}}})", actionDescriptor.Parameters[1].Name);

						if (!pathTemplate.StartsWith(compareStartPath) || !pathTemplate.EndsWith(compareEndPath)) return null;

						actionName = pathTemplate.Substring(compareStartPath.Length, pathTemplate.Length - compareStartPath.Length - compareEndPath.Length);
					}

					// The basic assumption here is that the unresolved segment is a string that looks like this: 
					//      "SomeFunctionName(someParameterValue)"
					// So the code below attempts to match the right action and strip out the parameter value
					// with simple string manipulations.
					unresolvedPattern = unresolvedPattern.Replace(actionName, string.Empty);

					if (unresolvedPattern.StartsWith("("))
					{
						unresolvedPattern = unresolvedPattern.Substring(1);
					}
					else
					{
						continue;
					}

					if (unresolvedPattern.EndsWith(")"))
					{
						unresolvedPattern = unresolvedPattern.Substring(0, unresolvedPattern.Length - 1);
					}
					else
					{
						continue;
					}

					// What is left in unresolvedPattern should be a single value representing the 
					// second key value for the function.  Use what we know to set the route data values
					// and return this matching action descriptor.
					routeContext.RouteData.Values["odataPath"] = odataPath;
					routeContext.RouteData.Values[actionDescriptor.Parameters[0].Name] = keyParentId;
					routeContext.RouteData.Values[actionDescriptor.Parameters[1].Name] = unresolvedPattern;
					routeContext.RouteData.Values[ODataRouteConstants.Key] = keyParentId;
					routeContext.RouteData.Values[ODataRouteConstants.RelatedKey] = unresolvedPattern;
					return new[] { actionDescriptor };
				}
			}
			//else if ("~/entityset/key/navigation")

			return null;
		}
	}
}
