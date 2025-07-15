using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Models;

namespace Lims.WebAPI.Profiles
{
    public class AutoMapperProFile : MapperConfigurationExpression
    {
        public AutoMapperProFile()
        {
            CreateMap<SampleModel, SampleDto>()
                .ReverseMap();          
            CreateMap<ItemModel, ItemDto>()
                .ForMember(dest => dest.ExecuteStandard, opt => opt.MapFrom(src => src.ProductStandard.ExecuteStandard))              
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductStandard.ProductType))
                .ForMember(dest => dest.AppointTime, opt => opt.MapFrom(src => src.AppointTime.ToLocalTime()))
                .ForMember(dest => dest.ProductClass, opt => opt.MapFrom(src => src.ProductStandard.ProductClass))
                .ForMember(dest => dest.ProductForm, opt => opt.MapFrom(src => src.ProductStandard.ProductForm))
                .ForMember(dest => dest.ExecuteStandard, opt => opt.MapFrom(src => src.ProductStandard.ExecuteStandard))
                .ForMember(dest => dest.TestMethod, opt => opt.MapFrom(src => src.MethodStandard.TestMethod))
                .ForMember(dest => dest.RoundRule, opt => opt.MapFrom(src => src.MethodStandard.RoundRule))
                .ForMember(dest => dest.SampleFormOrState, opt => opt.MapFrom(src => src.ProductStandard == null ? src.Sample.SampleState : src.ProductStandard.ProductForm))
                .ReverseMap();

            CreateMap<SubItemModel, SubItemDto>().ReverseMap();
            CreateMap<ProductStandardModel, ProductStandardDto>().ReverseMap();
            CreateMap<MethodStandardModel, MethodStandardDto>().ReverseMap();
            CreateMap<SubItemStandardModel, SubItemStandardDto>().ReverseMap();
            CreateMap<LoggerModel, LoggerDto>().ReverseMap();
            CreateMap<UserModel, UserDto>().ReverseMap();
            CreateMap<ReagentModel, ReagentDto>().ReverseMap();
        }
    }   
}
