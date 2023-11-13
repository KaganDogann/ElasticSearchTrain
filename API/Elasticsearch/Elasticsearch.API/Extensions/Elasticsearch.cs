using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.API.Extensions;

public static class Elasticsearch
{
    public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
    {
        var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"]!)); // Tek bir node da çalıştığım için SingleNode seçtik sanırım.
        var settings = new ConnectionSettings(pool);
        var client = new ElasticClient(settings);
        services.AddSingleton(client); // program ayağa kalkarken instance oluşturdu ya program ayakta olduğu sürece 2 sene boyunca kalacak o instance
    }
}
