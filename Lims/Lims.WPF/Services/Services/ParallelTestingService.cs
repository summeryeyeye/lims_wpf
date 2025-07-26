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

        
    }
}
