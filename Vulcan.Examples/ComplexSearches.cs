using EPiServer.Core;
using Nest;
using System;
using TcbInternetSolutions.Vulcan.Core;
using TcbInternetSolutions.Vulcan.Core.Extensions;

namespace Vulcan.Examples
{
    public class ComplexSearches
    {
        private IVulcanHandler _VulcanHandler;        

        public ComplexSearches()
        {
            _VulcanHandler = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IVulcanHandler>();            
        }

        public void MultipleFieldSearches(string searchText)
        {
            var searchTextQuery = new QueryContainerDescriptor<IContent>().Bool(b =>
                b.Must(must =>
                    must.SimpleQueryString(sqs => sqs.Fields(f => f
                                    .AllAnalyzed()
                                    .Field($"{VulcanFieldConstants.MediaContents}.content")
                                    .Field($"{VulcanFieldConstants.MediaContents}.content_type"))
                        .Query(searchText)
                        .Analyzer("default")
                    ) &&
                    must.Term(t => t.Field(xf => xf.IsDeleted).Value(false)) &&
                    must.DateRange(dr => dr.LessThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StartPublish)) &&
                    must.DateRange(dr => dr.GreaterThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StopPublish))
                )
            // NOTE: Can also filter, but doesn't score them this way
            //.Filter(f => 
            //        f.Term(t => t.Field(xf => xf.IsDeleted).Value(false)) &&
            //        f.DateRange(dr => dr.LessThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StartPublish)) &&
            //        f.DateRange(dr => dr.GreaterThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StopPublish))
            //)
            );

            var client = _VulcanHandler.GetClient();
            var results = client.Search<IContent>(r => r.Query(q => searchTextQuery));
        }
    }
}
