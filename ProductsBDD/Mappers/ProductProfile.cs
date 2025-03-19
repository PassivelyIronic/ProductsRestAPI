using AutoMapper;
using ProductAPI.Dtos;
using ProductAPI.Models;

namespace ProductAPI.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }

}
