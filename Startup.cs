using AutoMapper;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData;
using PacsNext.WebApi.Support;
using System.Collections.Generic;
using WidgetWebAPI.Domain;
using WidgetWebAPI.Support;

namespace WidgetWebAPI
{
	public class Startup
	{
		#region Constructor(s)

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			CurrentEnvironment = environment;
		}

		#endregion

		#region Private Properties

		private IConfiguration Configuration { get; }
		private IWebHostEnvironment CurrentEnvironment { get; }

		#endregion

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOData();
			services.AddControllers();
			services.AddDbContext<Data.WidgetDBContext>(optionsBuilder =>
			{
				if (!optionsBuilder.IsConfigured)
				{
					optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"));
				}
			});

			services
				.AddAutoMapper(this.GetType().Assembly)
				.AddScoped<DomainContext, DomainContext>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			// Add "UseODataBatching()" before "UseRouting()" to support OData $batch.
			app.UseODataBatching();
			app.UseRouting();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.MapODataRoute("odata", "odata", new PropertyModelBuilder(app.ApplicationServices).GetEdmModel());
				endpoints.MapODataRoute("odata", "odata",
					b =>
					{
						b.AddService(Microsoft.OData.ServiceLifetime.Singleton,
							sp => new DomainModelBuilder(app.ApplicationServices).GetEdmModel());

						b.AddService<ODataDeserializerProvider>(Microsoft.OData.ServiceLifetime.Singleton,
							sp => new EntityReferenceODataDeserializerProvider(sp));

						b.AddService<ODataBatchHandler>(Microsoft.OData.ServiceLifetime.Singleton,
							sp => new TransactionedODataBatchHandler());

						b.AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton,
							sp =>
							{
								var routingConventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting("odata", endpoints.ServiceProvider);
								// THIS DOES NOT WORK:
								// With attribute routing in place, this class no longer works.  Running the app immediately throws an
								// exception if I uncomment and attempt to use the [ODataRoute] attribute like this:
								// [ODataRoute("Supplier({keySupplierId})/Part({keyPartId})")]

								//routingConventions.Insert(0, new NavigationIndexRoutingConvention());
								//routingConventions.Add(new NavigationIndexRoutingConvention());
								return routingConventions;
							});
					});
			});
		}
	}
}
