using Lims.Common.Dtos;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class ReagentService : BaseService<ReagentDto>, IReagentService
    {
        public ReagentService() : base("Reagents")
        {
        }
    }
}
