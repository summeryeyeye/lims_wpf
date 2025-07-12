using Lims.Common.Dtos;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class ProductStandardService : BaseService<ProductStandardDto>, IProductStandardService
    {
        public ProductStandardService() : base("ProductStandards")
        {
        }
    }
}
