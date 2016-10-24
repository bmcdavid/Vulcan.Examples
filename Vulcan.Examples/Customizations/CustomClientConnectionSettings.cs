using Elasticsearch.Net;
using Nest;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using TcbInternetSolutions.Vulcan.Core.Implementation;

namespace Vulcan.Examples.Customizations
{
    /// <summary>
    /// Options found here: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/connection-pooling.html
    /// <para> This example uses sniffing connections, remember to register with structuremap as seen in RegisterCustomizations</para>
    /// </summary>
    public class CustomClientConnectionSettings : VulcanClientConnectionSettings
    {
        /// <summary>
        /// override the default to use a sniffing connection pool
        /// </summary>
        /// <returns></returns>
        protected override ConnectionSettings CommonSettings()
        {
            CompilationSection section = ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            bool isDebugMode = section != null ? section.Debug : false;
            var url = ConfigurationManager.AppSettings["VulcanUrl"];
            var Index = ConfigurationManager.AppSettings["VulcanIndex"];

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(url) || url == "SET THIS")
            {
                throw new Exception("You need to specify the Vulcan Url in AppSettings");
            }

            if (string.IsNullOrWhiteSpace(Index) || string.IsNullOrWhiteSpace(Index) || Index == "SET THIS")
            {
                throw new Exception("You need to specify the Vulcan Index in AppSettings");
            }

            var urlParts = url.Split(':');
            var port = urlParts.Length == 3 ? int.Parse(urlParts[2]) : 9200;

            var uris = Enumerable.Range(port, 5).Select(p => new Uri(urlParts[0] + ":" + urlParts[1] + ":" + p));
            var connectionPool = new SniffingConnectionPool(uris);
            //new SingleNodeConnectionPool(new Uri(url));
            var settings = new ConnectionSettings(connectionPool, s => new VulcanCustomJsonSerializer(s));
            var username = ConfigurationManager.AppSettings["VulcanUsername"];
            var password = ConfigurationManager.AppSettings["VulcanPassword"];

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                settings.BasicAuthentication(username, password);
            }

            bool enableCompression = false;
            bool.TryParse(ConfigurationManager.AppSettings["VulcanEnableHttpCompression"], out enableCompression);

            // Enable bytes to be retrieved in debug mode
            settings.DisableDirectStreaming(isDebugMode);

            // Enable compression
            settings.EnableHttpCompression(enableCompression);

            return settings;
        }
    }
}
