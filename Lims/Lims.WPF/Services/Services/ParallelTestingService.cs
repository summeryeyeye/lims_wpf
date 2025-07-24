using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.WPF.Services.Services
{
    public class ParallelTestingService : BaseService<ParallelTestingDto>, IParallelTestingService
    {
        public ParallelTestingService() : base("ParallelTestings")
        {
        }

        public async Task<ApiResponse<List<ParallelTestingDto>>> GetParallelTestingsByParentIdAsync(ParallelTestingFilterParam param)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetParallelTestingsByParentId?ParentId={param.ParentKey}"
            };
            return await client.ExecuteAsync<List<ParallelTestingDto>>(request);
        }
    }
}
