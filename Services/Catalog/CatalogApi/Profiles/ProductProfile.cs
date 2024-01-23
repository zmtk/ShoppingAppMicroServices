using AutoMapper;
using CatalogApi.Dtos;
using CatalogApi.Models;

namespace CatalogApi.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Product, ProductReadDto>();
        CreateMap<Product, UpdateBasketDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));

        CreateMap<ProductCreateDto, Product>();

        CreateMap<Store, StoreReadDto>();
        CreateMap<StoreCreateDto, Store>();
        CreateMap<ProductReadDto,UpdateBasketPriceDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NewPrice, opt => opt.MapFrom(src => src.Price));
    }
}