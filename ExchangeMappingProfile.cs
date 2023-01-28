using AutoMapper;
using ExchangeRate.Entities;
using ExchangeRate.Model;

namespace ExchangeRate;

public class ExchangeMappingProfile : Profile
{
    public ExchangeMappingProfile()
    {
        CreateMap<Cache, ExchangeRateDTO>().ReverseMap();
    }
}