namespace ExchangeRate.Model;

public class ExchangeRateDTO
{
    public string FirstCurrency { get; set; }
    public string SecondCurrency { get; set; }
    public DateTime Date { get; set; }
    public decimal ExchangeRateValue { get; set; }
}