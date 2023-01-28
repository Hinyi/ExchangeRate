using ExchangeRate.Model;

namespace ExchangeRate.Interface;

public interface IExchangeRateService
{
    Task<IEnumerable<ExchangeRateDTO>> GetExchangeRate(KeyValuePair<string, string> currencyCodes,
        DateTime startDate, DateTime endDate);
}