using AutoMapper;
using BasketApi.Models;

namespace BasketApi.Profiles;

public class BasketProfile : Profile
{
    public BasketProfile()
    {
        
        CreateMap<AddToBasketRequest, BasketItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

        CreateMap<Basket, GetBasketResponse>()
            .ForMember(dest => dest.BasketTotal, opt => opt.MapFrom(src => src.BasketTotal))
            .ForMember(dest => dest.BasketItems, opt => opt.MapFrom(src => src.BasketItems));
        
        CreateMap<BasketItem, GetBasketItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Inactive, opt => opt.MapFrom(src => src.Inactive));
    }
}