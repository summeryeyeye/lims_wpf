using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lims.WebAPI.Controllers
{
    public class ProductStandardsController : MyBaseController<ProductStandardModel, ProductStandardDto>
    {
        private readonly IProductStandardService productStandardService;

        public ProductStandardsController(IProductStandardService productStandardService,IMapper mapper):base(mapper, (BaseService<ProductStandardModel>)productStandardService)
        {
            this.productStandardService = productStandardService;
        }
    }
}
