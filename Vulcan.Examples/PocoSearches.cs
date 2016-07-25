using System.Web;
using TcbInternetSolutions.Vulcan.Core;
using TcbInternetSolutions.Vulcan.Core.Extensions;
using Vulcan.Examples.Models.Pocos;

namespace Vulcan.Examples
{
    public class PocoSearches
    {
        private IVulcanHandler _VulcanHandler;        

        public PocoSearches()
        {
            _VulcanHandler = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IVulcanHandler>();
        }

        public void SearchPostsPocos()
        {
            var client = _VulcanHandler.GetClient();
            var context = HttpContext.Current;
            int currentPage = 1;
            int maxResults = 10;
            string searchText = "dolor";

            var pocoTest =  client.PocoSearch<Post>(r => r
                .Skip((currentPage - 1) * maxResults)
                .Take(maxResults)
                .Query(q => q.SimpleQueryString(sqs => sqs.Fields(f => f.FieldAnalyzed(p => p.Title)).Query(searchText)))
                );

            // For debugging request and response body, set "system.web/compilation" debug to true!
            if (context.IsDebuggingEnabled && pocoTest.ApiCall.RequestBodyInBytes != null && pocoTest.ApiCall.ResponseBodyInBytes != null)
            {
                var requestString = System.Text.Encoding.UTF8.GetString(pocoTest.ApiCall.RequestBodyInBytes);
                var responseString = System.Text.Encoding.UTF8.GetString(pocoTest.ApiCall.ResponseBodyInBytes);
            }
}
    }
}
