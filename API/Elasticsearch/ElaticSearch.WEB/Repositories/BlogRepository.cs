using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElaticSearch.WEB.Models;
using System.Collections.Immutable;

namespace ElaticSearch.WEB.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _client;

        private const string IndexName = "blog";

        public BlogRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _client.IndexAsync(newBlog, x => x.Index(IndexName).Id(Guid.NewGuid().ToString()));

            //fast fail, başarısızsa hemen geri dön. 
            if (!response.IsValidResponse) return null;

            newBlog.Id = response.Id;

            return newBlog;
        }

        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            //title a göre ilk olarak hemde content e göre arama yapmak istiyorum

            // title => full Text
            // content => full Text

            List<Action<QueryDescriptor<Blog>>> ListQuery = new();

            Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll();

            Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(m => m.Field(f => f.Content).Query(searchText));

            Action<QueryDescriptor<Blog>> titleMatchBoolPrefix = (q) => q.MatchBoolPrefix(m => m.Field(f => f.Content).Query(searchText));

            if (string.IsNullOrEmpty(searchText))
            {
                ListQuery.Add(matchAll);
            }
            else
            {
                ListQuery.Add(matchContent);
                ListQuery.Add(titleMatchBoolPrefix);
            }


            var result = await _client.SearchAsync<Blog>(s => s
       .Index(IndexName)
           .Size(1000)
               .Query(q => q
                   .Bool(b => b
                       .Should(ListQuery.ToArray()))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToList();
        }
    }
}
