using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using TcbInternetSolutions.Vulcan.Core;
using Vulcan.Examples.Models.Pages;

namespace Vulcan.Examples.Indexing
{
    public class IndexContentType
    {
        private Injected<IVulcanHandler> _VulcanHandler { get; }

        private Injected<IContentLoader> _ContentLoader { get; }

        public void IndexAllGenericPages()
        {
            // NOTE this is only for adding new mapping fields, if the model has any property changes from existing, a full index must be done!
            var allGenericPageReferences = new ContentUtility().GetContentByType<GeneralPageData>(scope: ContentReference.StartPage);

            foreach (var reference in allGenericPageReferences)
            {
                GeneralPageData content;

                if (_ContentLoader.Service.TryGet<GeneralPageData>(reference, out content))
                {
                    _VulcanHandler.Service.IndexContentEveryLanguage(content);
                }
            }
        }
    }
}