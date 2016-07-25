using RestSharp;
using System.Collections.Generic;
using System.Linq;
using TcbInternetSolutions.Vulcan.Core;
using Vulcan.Examples.Models.Pocos;

namespace Vulcan.Examples.Indexers
{
    // POCOs based on https://jsonplaceholder.typicode.com/posts

    public class PostPocoIndexer : IVulcanPocoIndexer
    {
        /// <summary>
        /// Only used to hold sample data from web service to get a total count.
        /// </summary>
        private static List<Post> Posts = new List<Post>();

        public PostPocoIndexer()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (Posts.Count == 0)
            {
                var client = new RestClient("https://jsonplaceholder.typicode.com");
                var request = new RestRequest("posts", Method.GET);

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content; // raw content as string
                Posts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Post>>(content);
            }
        }

        public string IndexerName => nameof(PostPocoIndexer);

        public int PageSize => 20;

        /// <summary>
        /// This is just a sample with an API that doesn't support paging, do not grab an entire set of data to get total count in production code.
        /// </summary>
        public long TotalItems => Posts.Count;

        /// <summary>
        /// Determines if this poco indexing should occur in the default 'Vulcan Index Content' scheduled job. Set to false, if you want to index on your
        /// </summary>
        public bool IncludeInDefaultIndexJob => true;

        /// <summary>
        /// Used to determine ID field of POCO object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string GetItemIdentifier(object o)
        {
            var poco = o as Post;

            return poco.Id.ToString();
        }

        /// <summary>
        /// Gets a page of POCO objects to batch index.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<object> GetItems(int page, int pageSize)
        {
            var range = Posts.Skip((page - 1) * pageSize).Take(pageSize);

            return range;
        }
    }
}