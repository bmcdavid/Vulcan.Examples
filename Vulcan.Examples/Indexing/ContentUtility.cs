using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Vulcan.Examples.Indexing
{
    public class ContentUtility
    {
        private Injected<IContentTypeRepository> _ContentTypeRepository { get; }

        private Injected<IContentLoader> _ContentLoader { get; }

        private Injected<IDatabaseHandler> _DbHandler { get; }

        /// <summary>
        /// Gets a list of content references by content type. Optionally filters by scope/parent and categories.
        /// <para>IMPORTANT: This does not filter by expired or deleted content.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope">Optional scope/parent filter</param>
        /// <param name="categoryFilter">Optional category list filter</param>
        /// <param name="recursive">Adds wildcard to scope path.</param>
        /// <returns></returns>
        public virtual IEnumerable<ContentReference> GetContentByType<T>(ContentReference scope = null, CategoryList categoryFilter = null, bool recursive = true) where T : IContent
        {
            List<Guid> contentGuids = new List<Guid>(200);
            string scopeFilter = string.Empty;

            if (!ContentReference.IsNullOrEmpty(scope))
            {
                List<string> contentPaths = new List<string>();

                var parents = _ContentLoader.Service.GetAncestors(scope).Select(x => x.ContentLink).ToList();
                parents.Reverse();
                parents.Add(scope);

                scopeFilter = "." + string.Join(".", parents.Select(x => x.ID)) + (recursive ? ".%" : ".");
            }

            using (var sqlConnection = _DbHandler.Service.DbFactory.CreateConnection())
            {
                sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["EPiServerDB"].ConnectionString;
                string sqlCommand = "SELECT ContentGUID FROM tblContent where fkContentTypeID = @contentTypeID";

                if (!string.IsNullOrWhiteSpace(scopeFilter))
                    sqlCommand += " AND ContentPath LIKE @contentPath";

                if (categoryFilter?.Count > 0)
                    sqlCommand += string.Format(" AND pkID IN (SELECT fkContentID from tblContentCategory where fkCategoryID IN ({0}))", string.Join(",", categoryFilter));

                using (var command = sqlConnection.CreateCommand())
                {
                    var contentType = _ContentTypeRepository.Service.Load<T>();
                    var contentTypeIdParam = command.CreateParameter();
                    contentTypeIdParam.ParameterName = "@contentTypeID";
                    contentTypeIdParam.Value = contentType.ID;
                    command.Parameters.Add(contentTypeIdParam);

                    if (!string.IsNullOrWhiteSpace(scopeFilter))
                    {
                        var pathParam = command.CreateParameter();
                        pathParam.ParameterName = "@contentPath";
                        pathParam.Value = scopeFilter;
                        command.Parameters.Add(pathParam);
                    }

                    command.CommandText = sqlCommand;
                    sqlConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contentGuids.Add(reader.GetGuid(0));
                        }
                    }
                }
            }

            if (contentGuids.Count == 0)
                yield break;

            foreach (var guid in contentGuids)
                yield return PermanentLinkUtility.FindContentReference(guid);
        }
    }
}