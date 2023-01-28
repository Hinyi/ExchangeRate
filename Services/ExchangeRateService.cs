using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using ExchangeRate.Entities;
using ExchangeRate.Interface;
using ExchangeRate.Model;
using Microsoft.EntityFrameworkCore;
using Org.Sdmx.Resources.SdmxMl.Schemas.V20.generic;
using Org.Sdmx.Resources.SdmxMl.Schemas.V21.Message;

namespace ExchangeRate.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ExchangeRateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(IHttpClientFactory httpClientFactory, ExchangeRateDbContext dbContext,
        IMapper mapper, ILogger<ExchangeRateService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ExchangeRateDTO>> GetExchangeRate(KeyValuePair<string, string> currencyCodes,
        DateTime startDate, DateTime endDate)
    { 
        var httpClient = _httpClientFactory.CreateClient("ECB_SDMX");

        var startDateString = startDate.ToString("yyyy-MM-dd");
        var endDateString = endDate.ToString("yyyy-MM-dd");
  
        XmlDocument currencyRate = new XmlDocument();
        var stringUrl = $"https://sdw-wsrest.ecb.europa.eu/service/data/EXR/"+
                        $"D.{currencyCodes.Key}.{currencyCodes.Value}.SP00.A" +
                        $"?startPeriod={startDateString}" +
                        $"&endPeriod={endDateString}" +
                        $"&detail=dataonly";
        
        currencyRate.Load(stringUrl); 
 
        XmlNodeList nodeList;
        XmlNodeList nodeList1;
        XmlNode root = currencyRate.DocumentElement.FirstChild.NextSibling.FirstChild.FirstChild;
        XmlNode root1 = currencyRate.DocumentElement;
        
        string xpath = "/GenericData/DataSet";
        XmlNode locationNode = currencyRate.SelectSingleNode(xpath); 
        
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(currencyRate.NameTable);  
        nsmgr.AddNamespace("bk", $"http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic");  

        
        nodeList = root.SelectNodes("descendant::bk:Value",nsmgr);
        // nodeList1 = root1.SelectNodes("//bk:Value",nsmgr);
        nodeList1 = root1.SelectNodes("//bk:Obs",nsmgr);

        var curr = root.SelectSingleNode("bk:Value[@id='CURRENCY']",nsmgr).Attributes["value"].Value;
        var curr1 = root.SelectSingleNode("bk:Value[@id='CURRENCY_DENOM']",nsmgr).Attributes["value"].Value;

        var cache = new List<Cache>();
        
        foreach (XmlNode node in nodeList1)
        {
            var a = node.FirstChild.Attributes["value"].Value;
            var b = decimal.Parse(node.FirstChild.NextSibling.Attributes["value"].Value.Replace(".",","));
            
            cache.Add(new Cache()
            {
                FirstCurrency = curr,
                SecondCurrency = curr1,
                Date = DateTime.Parse(node.FirstChild.Attributes["value"].Value),
                ExchangeRateValue = decimal.Parse(node.FirstChild.NextSibling.Attributes["value"].Value.Replace(".",","))
            });
        }

        XDocument doc = XDocument.Load(stringUrl);
        var result = doc.Descendants("SeriesKey").Select(x => new
        {
            a = x.Attributes("id")
        }).ToList();

        XmlElement result_id = currencyRate.DocumentElement;

        var result1 = new List<Cache>();
        return _mapper.Map<List<ExchangeRateDTO>>(result1);
    }
}