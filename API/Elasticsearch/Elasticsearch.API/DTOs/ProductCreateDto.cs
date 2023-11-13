using Elasticsearch.API.Models;

namespace Elasticsearch.API.DTOs
{
    public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto Feature) //record immutable olarak kullnabiliyoruz, bu şekilde kullanınca bu class'tan bir nesne oluşturunca onun üretilien nesne örneğinin property'lerini değiştiremiyoruz.üretlkdikten sonra değiştirilemez, functional programing
    {



        public Product CreateProduct() // burada ne yaptık ilgili map işlemini buraya taşıdım ilgil kodu ilgili sınıfa yaklaştırdık bağlılığını naptık artırdık.
        {
            return new Product
            {
                Name = Name,
                Price = Price,
                Stock = Stock,
                CreatedDate = DateTime.Now,
                Feature = new ProductFeature() { Width = Feature.Width, Height = Feature.Height, Color = (EColor)int.Parse(Feature.Color) }
            };
        }


    }
}
// bu record intermediate lang. e döndüğü anda zaten class'a çevriliyor derlendiği zaman prop oalrak gelecekelr yukarıya