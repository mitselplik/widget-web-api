using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using System;
using System.Linq;
using WidgetWebAPI.Models;

namespace WidgetWebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// See note on https://devblogs.microsoft.com/odata/experimenting-with-odata-in-asp-net-core-3-1/
			// Disabling end-point routing isn't ideal, but is required for the current implementation of OData 
			// (7.4.0 as of this comment).  As OData is further updated, this will change.
			//services.AddControllers();
			services.AddControllers(mvcOoptions => mvcOoptions.EnableEndpointRouting = false);

			services.AddDbContext<Models.WidgetDBContext>(optionsBuilder =>
			{
				if (!optionsBuilder.IsConfigured)
				{
					optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"));
				}
			});

			services.AddOData();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();

			// Again, this is temporary due to current OData implementation.  See note above.
			//app.UseEndpoints(endpoints =>
			//{
			//    endpoints.MapControllers();
			//});

			//IRouteBuilder theNaughtyRouteBuilder = null;

			app.UseMvc(routeBuilder =>
			{
				// the following will not work as expected
				// BUG: https://github.com/OData/WebApi/issues/1837
				// routeBuilder.SetDefaultODataOptions(new ODataOptions { UrlKeyDelimiter = Microsoft.OData.ODataUrlKeyDelimiter.Parentheses });
				var options = routeBuilder.ServiceProvider.GetRequiredService<ODataOptions>();
				options.UrlKeyDelimiter = Microsoft.OData.ODataUrlKeyDelimiter.Parentheses;

				routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());

				//theNaughtyRouteBuilder = routeBuilder;
			});

			//IRouter theNaughtyRouter = theNaughtyRouteBuilder.Routes[0];
			//theNaughtyRouteBuilder.Routes.RemoveAt(0);
			//theNaughtyRouteBuilder.Routes.Add(theNaughtyRouter);
			//app.UseRouter(theNaughtyRouteBuilder.Build());
			
		}

		private IEdmModel GetEdmModel()
		{
			var builder = new ODataConventionModelBuilder();
			builder.Namespace = "WidgetData";   // Hide Model Schema from $metadata
			builder.EntitySet<Widget>("Widget").EntityType
				.HasKey(r => r.WidgetId)
				.Filter()   // Allow for the $filter Command
				.Count()    // Allow for the $count Command
				.Expand()   // Allow for the $expand Command
				.OrderBy()  // Allow for the $orderby Command
				.Page()     // Allow for the $top and $skip Commands
				.Select();  // Allow for the $select Command;

			return builder.GetEdmModel();
		}
	}
}
