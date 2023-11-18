using Elasticsearch.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ECommerceController : ControllerBase
{
    private readonly ECommerceRepository _repository;

    public ECommerceController(ECommerceRepository repository)
    {
        _repository = repository;
    }

    [HttpGet()]
    public async Task<IActionResult> TermQuery(string customerFirstName)
    {
        return Ok(await _repository.TermQuery(customerFirstName));
    }

    [HttpGet()]
    public async Task<IActionResult> TermsQuery([FromQuery] List<string> customersFirstName)
    {
        return Ok(await _repository.TermsQuery(customersFirstName));
    }

    [HttpGet()]
    public async Task<IActionResult> PrefixQuery([FromQuery] string CustomerFullName)
    {
        return Ok(await _repository.PrefixQuery(CustomerFullName));
    }

    [HttpGet()]
    public async Task<IActionResult> RangeQuery([FromQuery] double fromPrice, double toPrice)
    {
        return Ok(await _repository.RangeQueryAsync(fromPrice, toPrice));
    }

    [HttpGet()]
    public async Task<IActionResult> MatchAllQuery()
    {
        return Ok(await _repository.MatchAllAsync());
    }

    [HttpGet()]
    public async Task<IActionResult> PagianationQuery(int page, int pageSize)
    {
        return Ok(await _repository.PaginationAsync(page, pageSize));
    }

    [HttpGet()]
    public async Task<IActionResult> WildcardQuery(string customerFullName)
    {
        return Ok(await _repository.WildcardQueryAsync(customerFullName));
    }

    [HttpGet()]
    public async Task<IActionResult> FuzzyQueryAsync(string customerFirstName)
    {
        return Ok(await _repository.FuzzyQueryAsync(customerFirstName));
    }

    [HttpGet()]
    public async Task<IActionResult> MatchQueryFullText(string categoryName)
    {
        return Ok(await _repository.MatchQueryFullTextAsync(categoryName));
    }

    [HttpGet()]
    public async Task<IActionResult> MatchBooleanPrefixFullText(string customerFullName)
    {
        return Ok(await _repository.MatchBooleanPrefixFullTextAsync(customerFullName));
    }

    [HttpGet()]
    public async Task<IActionResult> MatchPhraseFullText(string customerFullName)
    {
        return Ok(await _repository.MatchPhraseFullTextAsync(customerFullName));
    }

    [HttpGet()]
    public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string menufacturer)
    {
        return Ok(await _repository.CompoundQueryExampleOneFullTextAsync(cityName, taxfulTotalPrice, categoryName, menufacturer));
    }

    [HttpGet()]
    public async Task<IActionResult> CompoundQueryExampleTwoFullTextAsync(string customerFullName)
    {
        return Ok(await _repository.CompoundQueryExampleTwoFullTextAsync(customerFullName));
    }

    [HttpGet()]
    public async Task<IActionResult> MultiMatchQueryFullText(string name)
    {
        return Ok(await _repository.MultiMatchQueryFullTextAsync(name));
    }
}
