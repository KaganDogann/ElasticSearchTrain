using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "products";

        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Product> SaveAsync(Product newProduct)
        {
            newProduct.CreatedDate = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x=>x.Index(indexName).Id(Guid.NewGuid().ToString()));

            //fast fail, başarısızsa hemen geri dön. 
            if (!response.IsSuccess()) return null;

            newProduct.Id = response.Id;

            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync() //ImmutableList => bu listede limse değişiklik yapamayacak anlamına gelir.
        {
            var result = await _client.SearchAsync<Product>(
                s => s.Index(indexName).Query(q=>q.MatchAll()));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(indexName));

            if(!response.IsSuccess()) return null;

            response.Source.Id = response.Id;

            var product = response.Source;

            return product;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto productUpdateDto)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName, productUpdateDto.Id, x => x.Doc(productUpdateDto));

            return response.IsSuccess();
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x=>x.Index(indexName));

            return response;
        }
    }
}
