using EPiServer.Core;
using System;
using TcbInternetSolutions.Vulcan.Core;
using TcbInternetSolutions.Vulcan.Core.Extensions;
using Vulcan.Examples.Models.Pocos;

namespace Vulcan.Examples
{
    public class RangeSearches
    {
        private IVulcanHandler _VulcanHandler;

        public RangeSearches()
        {
            _VulcanHandler = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IVulcanHandler>();
        }

        public void SearchDateRange()
        {
            var client = _VulcanHandler.GetClient();
            int currentPage = 1;
            int maxResults = 10;
            DateTime upperLimit = new DateTime(2017, 01, 01);
            DateTime lowerLimit = new DateTime(2016, 01, 01);

            var hits = client.SearchContent<IContent>(d => d
                .Skip((currentPage - 1) * maxResults)
                .Take(maxResults)
                .Fields(fs => fs.Field(p => p.ContentLink))
                .Query(q => q.DateRange(rs => rs.LessThan(upperLimit).GreaterThan(lowerLimit).Field(xf => (xf as IChangeTrackable).Changed))
            ));
        }

        public void SearchIntRange()
        {
            var client = _VulcanHandler.GetClient();            
            int currentPage = 1;
            int maxResults = 10;            

            var pocoTest = client.PocoSearch<Post>(r => r
               .Skip((currentPage - 1) * maxResults)
               .Take(maxResults)
               .Query(q => q.Range(rs => rs.LessThan(100).GreaterThanOrEquals(10).Field(xf => xf.UserId)))
            );
        }
    }
}
