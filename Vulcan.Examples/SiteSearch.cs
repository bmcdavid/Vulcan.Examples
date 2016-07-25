namespace Vulcan.Examples
{
    using EPiServer;
    using EPiServer.Core;
    using System;
    using System.Linq;
    using TcbInternetSolutions.Vulcan.Core;
    using TcbInternetSolutions.Vulcan.Core.Extensions;

    public class SiteSearch
    {
        private IVulcanHandler _VulcanHandler;
        private IContentLoader _ContentLoader;

        public SiteSearch()
        {
            _VulcanHandler = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IVulcanHandler>();
            _ContentLoader = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        /// <summary>
        /// Search all types
        /// </summary>
        public void SearchAllContent()
        {
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;
            var searchScope = new ContentReference[] { ContentReference.StartPage };

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, searchRoots: searchScope);            
            string searchInfo = $"Search for {searchText} found {siteHits.TotalHits}, on page {siteHits.Page}, page size {siteHits.PageSize} ran for {siteHits.ResponseContext.Took}  ms!";
            string typesInfo = string.Join(Environment.NewLine, siteHits.ResponseContext.Aggs?.Terms("types")?.Buckets?.Select(x => $"{x.Key} ({x.DocCount})"));
            string resultsInfo = string.Join(Environment.NewLine + Environment.NewLine, siteHits.Items.Select(x => x.Title + " " + x.Id + " " + x.Url + Environment.NewLine + x.Summary));

            return;
        }

        /// <summary>
        /// Only search within a content reference and descendents
        /// </summary>
        public void SearchSectionContent()
        {
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;

            // Example using first child of site
            var section = _ContentLoader.GetChildren<PageData>(ContentReference.StartPage).FirstOrDefault();

            // Can be multiple areas
            var searchScope = new ContentReference[] { section.ContentLink };

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, searchRoots: searchScope);
        }

        public void SearchOnlyMediaContent()
        {
            var typesToSearchFor = typeof(MediaData).GetSearchTypesFor();
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, includeTypes: typesToSearchFor);
        }
    }
}