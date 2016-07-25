using Newtonsoft.Json;

namespace Vulcan.Examples.Models.Pocos
{
    //POCO based on https://jsonplaceholder.typicode.com/posts

    public class Post
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}