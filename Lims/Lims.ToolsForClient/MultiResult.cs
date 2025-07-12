using Lims.Common.Dtos;

namespace Lims.ToolsForClient
{
    public class MultiResult
    {
        public List<ItemDto>? Items { get; set; } 
        public List<SampleDto>? Samples { get; set; }
    }
}
