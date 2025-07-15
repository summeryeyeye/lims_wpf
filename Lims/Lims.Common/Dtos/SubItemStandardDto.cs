namespace Lims.Common.Dtos
{
    public class SubItemStandardDto : BaseDto
    {
        public int Id
        {
            get; set;
        }

        public string? SubitemName
        {
            get; set;
        }

        public string? ParentNames
        {
            get; set;
        }

        public string? SubitemType
        {
            get; set;
        }
        public string? Substratum { get; set; }
        public string? BatchNumber { get; set; }
        public string? Campany { get; set; }
        public string? Instruments { get; set; }
    }
}
