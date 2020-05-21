using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace PacsNext.WebApi.Support
{
	/// <summary>
	/// An OData Batch Handler derived from <see cref="DefaultODataBatchHandler"/> that wraps the work being done 
	/// in a <see cref="TransactionScope"/> so that if any errors occur, the entire unit of work is rolled back.
	/// </summary>
	class TransactionedODataBatchHandler : DefaultODataBatchHandler
	{
		public override async Task ProcessBatchAsync(HttpContext context, RequestDelegate nextHandler)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				await base.ProcessBatchAsync(context, nextHandler);
			}
		}
	}
}
