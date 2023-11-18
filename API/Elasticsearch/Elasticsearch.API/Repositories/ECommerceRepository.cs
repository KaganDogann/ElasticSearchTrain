using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModel;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories;

public class ECommerceRepository
{
    private readonly ElasticsearchClient _client;

    public ECommerceRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    private const string indexName = "kibana_sample_data_ecommerce";

    #region TermLevelQuery
    public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
    {

        //1. yol
        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.Field("customer_first_name.keyword").Value(customerFirstName))));

        //2.yol
        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.CustomerFirstName.Suffix("keyword"), customerFirstName)));

        //3.yol
        var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true };
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));


        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
    {
        List<FieldValue> terms = new List<FieldValue>();
        customerFirstNameList.ForEach(x =>
        {
            terms.Add(x);
        });

        //1.yol
        //var termsQuery = new TermsQuery()
        //{
        //    Field = "customer_first_name.keyword",
        //    Terms = new TermsQueryField(terms.AsReadOnly())
        //};

        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

        //2.yol
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        .Query(q => q
        .Terms(t => t
        .Field(f => f.CustomerFirstName
        .Suffix("keyword"))
        .Terms(new TermsQueryField(terms.AsReadOnly())))));


        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        .Query(q => q
        .Prefix(p => p
        .Field(f => f.CustomerFullName
        .Suffix("keyword"))
        .Value(customerFullName))));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        .Query(q => q
        .Range(r => r.NumberRange(nr => nr
        .Field(f => f.TaxfulTotalPrice)
        .Gte(fromPrice)
        .Lte(toPrice)))));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MatchAllAsync()
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
                .Query(q => q
                    .MatchAll()));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PaginationAsync(int page, int pageSize)
    {
        var pageFrom = (page - 1)*pageSize;

        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(pageSize).From(pageFrom)
                .Query(q => q
                    .MatchAll()));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> WildcardQueryAsync(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
           .Query(q => q.
           Wildcard(w => w.
           Field(f => f.CustomerFullName.
           Suffix("keyword")).
           Wildcard(customerFullName))));
               

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string customerName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
           .Query(q => q
           .Fuzzy(fu => fu
           .Field(f => f.CustomerFirstName
           .Suffix("keyword"))
           .Value(customerName)
           .Fuzziness(new Fuzziness(1))))
           .Sort(sort => sort
           .Field(f => f.TaxfulTotalPrice, new FieldSort() { Order = SortOrder.Desc })));


        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }
    #endregion

    #region FullTextQuery
    public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string categoryName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
        .Query(q => q
        .Match(m => m
        .Field(f => f.Category)
        .Query(categoryName).Operator(Operator.And))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MatchBooleanPrefixFullTextAsync(string customerFullNmame)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
        .Query(q => q
        .MatchBoolPrefix(m => m
        .Field(f => f.CustomerFullName)
        .Query(customerFullNmame))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MatchPhraseFullTextAsync(string customerFullNmame)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
        .Query(q => q
        .MatchPhrase(m => m
        .Field(f => f.CustomerFullName)
        .Query(customerFullNmame))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOneFullTextAsync(string cityName, double taxfulTotalPrice, string categoryName, string menufacturer)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
            .Size(1000)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Term(t => t
                                .Field("geoip.city_name")
                                    .Value(cityName)))
                        .MustNot(mn => mn
                            .Range(r => r
                                .NumberRange(nr => nr
                                    .Field(f => f.TaxfulTotalPrice)
                                        .Lte(taxfulTotalPrice))))
                        .Should(s => s
                            .Term(t => t
                                .Field(f => f.Category.Suffix("keyword"))
                                    .Value(categoryName)))
                        .Filter(f => f
                            .Term(t => t
                                .Field("manufacturer.keyword")
                                    .Value(menufacturer))))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> CompoundQueryExampleTwoFullTextAsync(string customerFullName)
    {
        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(1000).Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));

        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
            .Size(1000)
                .Query(q => q
                    .Bool(b => b
                        .Should(m => m
                            .Match(m => m
                                .Field(f => f.CustomerFullName)
                                    .Query(customerFullName))
                            .Prefix(p => p
                                .Field(f => f.CustomerFullName.Suffix("keyword"))
                                    .Value(customerFullName))))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MultiMatchQueryFullTextAsync(string name)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
        .Index(indexName)
        .Query(q => q.MultiMatch(mm => mm.Fields(new Field("customer_first_name").And("customer_last_name").And("customer_full_name")).Query(name))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }
    #endregion
}