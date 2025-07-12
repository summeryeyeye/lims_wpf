namespace Lims.Common.Dtos
{
    public class SubItemStandardDto : BaseDto
    {
        public int Id
        {
            get; set;
        }

        public string? Name
        {
            get; set;
        }

        public string? ParentNames
        {
            get; set;
        }

        public string? Type
        {
            get; set;
        }
        public string? Substratum { get; set; }
        public string? BatchNumber { get; set; }
        public string? Campany { get; set; }
        public string? Instruments { get; set; }
    }
}
