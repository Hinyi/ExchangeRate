using ExchangeRate.Interface;
using ExchangeRate.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExchangeRateController : Controller
{
    private readonly IExchangeRateService _exchangeRateService;

    public ExchangeRateController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }
    
    [HttpGet]
    // [SwaggerOperation(Summary = "Compares and returns the exchange rates of the two given currencies: Key - Value from the given time range startDate - endDate (yyyy-MM-dd)")]
    [Authorize]
    public async Task<IEnumerable<ExchangeRateDTO>> GetExchangeRates([FromQuery] KeyValuePair<string, string> currencyCodes,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        return await _exchangeRateService.GetExchangeRate(currencyCodes, startDate, endDate);
    }
}